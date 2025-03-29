using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Serialization.Invocation;
using Remotr.Testing;

namespace Remotr;

public sealed class AggregateEntity<T> : IsolatedTransactionalGrain, IAggregateEntity<T> where T : new()
{

    private readonly ChildCqCreator<T> _cqCreator;

    /// <summary>
    /// Grain that manages all transactions involving this grain.
    /// </summary>
    private ComponentId ComponentId;
    private IAggregateRoot AggregateRoot;

    private Dictionary<Guid, Func<Task>> _deferredTransactionCallbacks = new();

    // Caches of transaction items and operations maintained by this grain.
    private readonly TransactionStateCache _stateCache;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public AggregateEntity()
    {
        _jsonSerializerOptions = ServiceProvider.GetRequiredService<JsonSerializerOptions>();
        _stateCache = new(_jsonSerializerOptions, GetAggregate, ServiceProvider);
        ComponentId = JsonSerializer.Deserialize<ComponentId>(this.GetPrimaryKeyString(), options: _jsonSerializerOptions)!;
        AggregateRoot = GrainFactory.GetGrain<IAggregateRoot>(ComponentId.AggregateId);
        var aggregateId = AggregateRoot.GetGrainId();
        _cqCreator = new ChildCqCreator<T>(
            ServiceProvider,
            this,
            this.GetGrainId(),
            GrainFactory,
            new InternalQueryFactory(
                GrainFactory,
                _jsonSerializerOptions,
                aggregateId
            ),
            new InternalCommandFactory(
                GrainFactory,
                _jsonSerializerOptions,
                aggregateId
            ),
            _stateCache,
            ComponentId,
            ComponentId.AggregateId,
            NotifyManagerOfTransactionParticipation,
            GetTransactionMetadata
        );
    }

    /// <summary>
    /// Random Guid set on activation. Used to know if a grain has been restarted during a transaction.
    /// </summary>
    private Guid AlivenessKey { get; set; }

    public override IAggregateRoot GetAggregate()
    {
        return AggregateRoot;
    }


    private bool notifiedManagerOfChanges = false;
    private TransactionMetadata TransactionMetadata = new();
    protected override TransactionMetadata GetTransactionMetadata()
    {
        return TransactionMetadata;
    }
    protected override void SetTransactionMetadata(TransactionMetadata transactionMetadata)
    {
        notifiedManagerOfChanges = false;
        TransactionMetadata = transactionMetadata;
    }

    /// <summary>
    /// TransactionId of last transaction this grain participated in.
    /// </summary>
    private Guid ParticipatingTransactionId
    {
        get
        {
            return TransactionMetadata.TransactionId;
        }
    }
    private DateTime ParticipatingTransactionTimestamp
    {
        get
        {
            return TransactionMetadata.TimeStamp;
        }
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        AlivenessKey = Guid.NewGuid();
    }

    protected async Task DeferredTransactionCallback(Func<Task> callback)
    {
        var callbackId = Guid.NewGuid();
        _deferredTransactionCallbacks.Add(callbackId, callback);
        await AggregateRoot.DeferEntityGrainTransactionCallback(this, callbackId);
    }

    public async Task HandleCallback(Guid callbackId)
    {
        _ = _deferredTransactionCallbacks.TryGetValue(callbackId, out var callback);
        if (callback is null)
        {
            return;
        }
        await callback();
        _deferredTransactionCallbacks.Remove(callbackId);
    }

    public Task<TransactionOperations> GetTransactionOperations()
    {
        // The list of transaction operations to return.
        var dataOperations = _stateCache.GetOperations(ParticipatingTransactionId);

        if (dataOperations == null || dataOperations.Count == 0)
        {
            throw new InvalidOperationException("No operations to commit which means this grain must have failed.");
        }

        // Return transaction operations.
        return Task.FromResult(new TransactionOperations
        {
            StateOperations = dataOperations,
        });
    }


    protected async Task ClearState()
    {
        _stateCache.ClearState(ComponentId.ItemId, ParticipatingTransactionId, ParticipatingTransactionTimestamp);
        await NotifyManagerOfTransactionParticipation();
    }

    protected async ValueTask NotifyManagerOfTransactionParticipation()
    {
        // Notify manager grain of transaction participation if not already done.
        if (!notifiedManagerOfChanges)
        {
            notifiedManagerOfChanges = true;
            await AggregateRoot.NotifyOfTransactionParticipation(ParticipatingTransactionId, this.GetGrainId(), AlivenessKey);
        }
    }


    public async Task<TOutput> ExecuteInterleaving<TOutput>(ExecutionStep<TOutput> execution)
    {
        return await Execute(execution);
    }


    public async Task<TOutput> ExecuteNotInterleaving<TOutput>(ExecutionStep<TOutput> execution)
    {
        return await Execute(execution);
    }

    private async Task<TOutput> Execute<TOutput>(ExecutionStep<TOutput> execution)
    {

        var testIdObj = RequestContext.Get("remotrTestId");
        if (testIdObj != null)
        {
            Guid testId = (Guid)testIdObj;
            _cqCreator.isTesting = true;
            _cqCreator.mockContainer = ServiceProvider.GetRequiredService<ICqMockContainerFactory>()
                    .GetContainer(testId);
        }
        execution.PassCqCreator(_cqCreator);
        return await execution.Run();
    }
}
