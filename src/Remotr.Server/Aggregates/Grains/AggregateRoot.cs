using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Concurrency;
using Remotr.Testing;

namespace Remotr;

public abstract class AggregateRoot<T> : Grain, IAggregateRoot, IIncomingGrainCallFilter
    where T : IAggregateRoot
{

    private readonly ManagerCqCreator<T> _managerCqCreator;

    public async Task Invoke(IIncomingGrainCallContext context)
    {
        ClearAllButTestContext();
        if (context.MethodName == "ExecuteQuery<T>" && (bool)context.Request.GetArgument(1)! != true)
        {
            DateTime readBeforeTimestamp;
            if (TransactionId is not null && !TransactionCompleted)
            {
                readBeforeTimestamp = TransactionTimestamp!.Value;
            }
            else
            {
                readBeforeTimestamp = UtcDateService.GetUtcDate();
            }
            RequestContext.Set("readOnly", true);
            RequestContext.Set("readBeforeTimestamp", readBeforeTimestamp);
        }

        await context.Invoke();
    }

    private string _grainTypeName;

    /// <summary>
    /// Persistent store that will be used to store the state of the items.
    /// </summary>
    private IPersistentStore PersistentStore { get; }

    /// <summary>
    /// Temporary cache for items that are updated during a transaction. Removed after updates are retrieved by transaction child grains.
    /// </summary>
    private IDictionary<string, string> Cache { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Id of the last transaction executed by this grain.
    /// </summary>
    private Guid? TransactionId { get; set; }
    /// <summary>
    /// Timestamp of the last transaction executed by this grain.
    /// </summary>
    private DateTime? TransactionTimestamp { get; set; }
    private bool TransactionCompleted { get; set; } = false;

    /// <summary>
    /// Maps grain ids to activation ids. Used to detect grain restarts during a transaction.
    /// </summary>
    private ParticipatingGrainIdStore ParticipatingGrainIdStore { get; } = new();

    private IUtcDateService UtcDateService { get; set; }

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public AggregateRoot(IPersistentStore persistentStore)
    {
        _grainTypeName = typeof(T).Name.ToString();
        _jsonSerializerOptions = ServiceProvider.GetRequiredService<JsonSerializerOptions>();
        UtcDateService = ServiceProvider.GetRequiredService<IUtcDateService>();
        PersistentStore = persistentStore;
        _managerCqCreator = new ManagerCqCreator<T>(
            ServiceProvider,
            this,
            this.GetGrainId(),
            GrainFactory,
            new InternalQueryFactory(
                GrainFactory,
                _jsonSerializerOptions,
                this.GetGrainId()
            ),
            new InternalCommandFactory(
                GrainFactory,
                _jsonSerializerOptions,
                this.GetGrainId()
            )
        );
    }

    public Task Wait()
    {
        return Task.CompletedTask;
    }

    public async Task DeferEntityGrainTransactionCallback<T>(IAggregateEntity<T> entityGrain, Guid callbackId)
        where T : new()
    {
        await StartTransaction(async () => await entityGrain.HandleCallback(callbackId));
    }

    public virtual async Task<Immutable<StateType>> GetState<StateType>(string itemId) where StateType : new()
    {
        // Check if item is in cache and get it if it is.
        var inCache = Cache.TryGetValue(itemId, out var value);

        // If in cache, remove from cache and return.
        if (inCache)
        {
            Cache.Remove(itemId);
            var x = typeof(StateType);
            return new Immutable<StateType>(JsonSerializer.Deserialize<StateType>(value!, options: _jsonSerializerOptions)!);
        }

        // If not in cache, get from persistent store and return.
        return new Immutable<StateType>(await PersistentStore.ReadItem<StateType>(itemId, this.GetGrainId()));
    }

    public Task NotifyOfTransactionParticipation(Guid transactionId, GrainId grainId, Guid activationId)
    {
        // If transaction ids don't match, throw exception.
        if (TransactionId != transactionId)
        {
            throw new InvalidOperationException("Must be called within the same transactional context.");
        }

        // Add grain id to participating grain id store.
        ParticipatingGrainIdStore.AddParticipatingGrain(transactionId, grainId, activationId);

        // Return completed task.
        return Task.CompletedTask;
    }

    protected async virtual Task<T> StartTransaction<T>(Func<Task<T>> transactionCallback)
    {
        if (RequestContext.Get("readOnly") is bool readOnly && readOnly)
        {
            throw new InvalidOperationException("Cannot execute a transaction in a read-only context.");
        }

        // Set new transaction id and add it to request context.
        var utcNow = UtcDateService.GetUtcDate();
        var transactionId = Guid.NewGuid();
        TransactionId = transactionId;
        TransactionTimestamp = utcNow;
        TransactionCompleted = false;
        RequestContext.Set("transactionId", TransactionId);
        RequestContext.Set("transactionTimestamp", utcNow);
        RequestContext.Set("ManagerId", this.GetGrainId());

        try
        {
            // Perform transaction callback.
            var returnValue = await transactionCallback();

            // Get transaction operations from all transaction child grains.
            var tasks = new List<Task<TransactionOperations>>();
            var participatingGrainIds = ParticipatingGrainIdStore.GetGrainIds(transactionId);
            if (participatingGrainIds.Count() == 0)
            {
                return returnValue;
            }
            foreach (var grainId in participatingGrainIds)
            {
                var grain = GrainFactory.GetGrain(grainId).AsReference<IHaveTransactionOperations>();
                tasks.Add(grain.GetTransactionOperations());
            }
            var allTransactionOperations = await Task.WhenAll(tasks);

            // Flatten all transaction operations into a single list.
            var batchedTransactionOperations = new List<TransactionStateOperation>();
            var participatingGrainIdsEnumerator = participatingGrainIds.GetEnumerator();
            foreach (var transactionOperations in allTransactionOperations)
            {
                participatingGrainIdsEnumerator.MoveNext();
                batchedTransactionOperations.AddRange(transactionOperations.StateOperations);
                var grainId = participatingGrainIdsEnumerator.Current;
            }

            // Execute transaction operations in persistent store.
            var updatedItems = await PersistentStore.ExecuteTransaction(batchedTransactionOperations, this.GetGrainId());

            // Update cache with updated items.
            foreach (var (itemId, itemJson) in updatedItems)
            {
                Cache[itemId] = itemJson;
            }

            // Clear transaction id in request context.
            RequestContext.Remove("transactionId");
            return returnValue;
        }
        finally
        {
            TransactionCompleted = true;
        }
    }

    protected async virtual Task StartTransaction(Func<Task> transactionCallback)
    {
        // Wrap transaction callback in a function that returns a bool.
        async Task<bool> boolTransactionCallback()
        {
            await transactionCallback();
            return true;
        }

        // Start transaction.
        await StartTransaction(boolTransactionCallback);
    }


    public async Task<Q> ExecuteCommand<Q>(ExecutionStep<Q> execution)
    {
        await CheckIfTest();
        execution.PassCqCreator(_managerCqCreator);
        var result = await StartTransaction(async () =>
        {
            return await execution.Run();
        });

        return result;
    }

    public async Task<Q> ExecuteQuery<Q>(ExecutionStep<Q> execution, bool forceConsistency)
    {
        var operationName = $"{_grainTypeName}.{execution.Name}";

        await CheckIfTest();
        execution.PassCqCreator(_managerCqCreator);
        var result = await execution.Run();

        return result;
    }

    private async ValueTask CheckIfTest()
    {
        var testIdObj = RequestContext.Get("remotrTestId");
        if (testIdObj != null)
        {
            Guid testId = (Guid)testIdObj;
            _managerCqCreator.isTesting = true;
            _managerCqCreator.mockContainer = ServiceProvider.GetRequiredService<ICqMockContainerFactory>()
                    .GetContainer(testId);

            // Ensuring this aggregate root grain is only being used by a single test.
            await GrainFactory.GetGrain<ITestManagerIsolatorGrain>(
                this.GetGrainId().ToString()
            ).EnsureTestIsolation(testId);
        }
    }

    private void ClearAllButTestContext()
    {
        var testIdObj = RequestContext.Get("remotrTestId");
        RequestContext.Clear();
        if (testIdObj != null)
        {
            RequestContext.Set("remotrTestId", testIdObj);
        }
    }
}
