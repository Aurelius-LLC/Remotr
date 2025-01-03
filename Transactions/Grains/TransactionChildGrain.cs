﻿using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Serialization.Invocation;
using Remotr.Testing;

namespace Remotr;

[MayInterleave(nameof(ArgHasInterleaveAttribute))]
public sealed class TransactionChildGrain<T> : IsolatedTransactionalGrain, ITransactionChildGrain<T> where T : new()
{

    private readonly ChildCqCreator<T> _cqCreator;

    /// <summary>
    /// Grain that manages all transactions involving this grain.
    /// </summary>
    private ComponentId ComponentId;
    private ITransactionManagerGrain TransactionManagerGrain;

    private Dictionary<Guid, Func<Task>> _deferredTransactionCallbacks = new();

    // Caches of transaction items and operations maintained by this grain.
    private readonly TransactionStateCache _stateCache;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public TransactionChildGrain()
    {
        _jsonSerializerOptions = ServiceProvider.GetRequiredService<JsonSerializerOptions>();
        _stateCache = new(_jsonSerializerOptions, GetManagerGrain, ServiceProvider);
        ComponentId = JsonSerializer.Deserialize<ComponentId>(this.GetPrimaryKeyString(), options: _jsonSerializerOptions)!;
        TransactionManagerGrain = GrainFactory.GetGrain<ITransactionManagerGrain>(ComponentId.ManagerGrainId);
        var managerGrainId = TransactionManagerGrain.GetGrainId();
        _cqCreator = new ChildCqCreator<T>(
            ServiceProvider,
            this,
            this.GetGrainId(),
            GrainFactory,
            new InternalQueryFactory(
                GrainFactory,
                _jsonSerializerOptions,
                managerGrainId
            ),
            new InternalCommandFactory(
                GrainFactory,
                _jsonSerializerOptions,
                managerGrainId
            ),
            _stateCache,
            ComponentId,
            ComponentId.ManagerGrainId,
            NotifyManagerOfTransactionParticipation,
            GetTransactionMetadata
        );
    }


    public static bool ArgHasInterleaveAttribute(IInvokable req)
    {
        if (req.GetMethodName() == "Execute<TOutput>" && (bool)req.GetArgument(1)! == true)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Random Guid set on activation. Used to know if a grain has been restarted during a transaction.
    /// </summary>
    private Guid AlivenessKey { get; set; }

    public override ITransactionManagerGrain GetManagerGrain()
    {
        return TransactionManagerGrain;
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
        await TransactionManagerGrain.DeferChildGrainTransactionCallback(this, callbackId);
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
            await TransactionManagerGrain.NotifyOfTransactionParticipation(ParticipatingTransactionId, this.GetGrainId(), AlivenessKey);
        }
    }


    public async Task<TOutput> Execute<TOutput>(ExecutionStep<TOutput> execution, bool interleave)
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
