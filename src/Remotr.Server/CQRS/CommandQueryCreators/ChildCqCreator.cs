using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Remotr.Testing;

namespace Remotr;

internal class ChildCqCreator<T> : ICqCreator, ICanReadState<T>, ICanUpdateState<T> where T : new()
{
    public bool isTesting = false;
    public ICqMockContainer? mockContainer;

    private readonly IServiceProvider _serviceProvider;
    private readonly IAddressable _addressable;
    private readonly GrainId _grainId;
    private readonly IGrainFactory _grainFactory;
    private readonly IInternalQueryFactory _queryFactory;
    private readonly IInternalCommandFactory _commandFactory;
    private readonly TransactionStateCache _stateCache;
    private readonly ComponentId _componentId;
    private readonly GrainId _managerGrainId;

    private readonly Func<ValueTask> _notifyManagerOfTransactionParticipation;
    private readonly Func<TransactionMetadata> _getTransactionMetadata;

    private Guid ParticipatingTransactionId
    {
        get
        {
            return _getTransactionMetadata().TransactionId;
        }
    }
    private DateTime ParticipatingTransactionTimestamp
    {
        get
        {
            return _getTransactionMetadata().TimeStamp;
        }
    }

    internal ChildCqCreator(
        IServiceProvider serviceProvider,
        IAddressable addressable,
        GrainId grainId,
        IGrainFactory grainFactory,
        IInternalQueryFactory queryFactory,
        IInternalCommandFactory commandFactory,
        TransactionStateCache stateCache,
        ComponentId componentId,
        GrainId managerGrainId,
        Func<ValueTask> notifyManagerOfTransactionParticipation,
        Func<TransactionMetadata> getTransactionMetadata
    )
    {
        _serviceProvider = serviceProvider;
        _addressable = addressable;
        _grainId = grainId;
        _grainFactory = grainFactory;
        _queryFactory = queryFactory;
        _commandFactory = commandFactory;
        _stateCache = stateCache;
        _componentId = componentId;
        _managerGrainId = managerGrainId;
        _notifyManagerOfTransactionParticipation = notifyManagerOfTransactionParticipation;
        _getTransactionMetadata = getTransactionMetadata;
    }



    public async ValueTask<T> GetState()
    {
        if (RequestContext.Get("readOnly") is bool readOnly && readOnly)
        {
            DateTime readBeforeTimestamp = (DateTime)RequestContext.Get("readBeforeTimestamp");
            return await _stateCache.GetReadOnlyState<T>(_componentId.ItemId, readBeforeTimestamp);
        }

        return await _stateCache.GetState<T>(_componentId.ItemId, ParticipatingTransactionId);
    }

    public async ValueTask UpdateState(T newItem)
    {
        if (RequestContext.Get("readOnly") is bool readOnly && readOnly)
        {
            throw new InvalidOperationException("Cannot update state in read-only context.");
        }

        _stateCache.UpdateState(_componentId.ItemId, newItem, ParticipatingTransactionId, ParticipatingTransactionTimestamp);

        await _notifyManagerOfTransactionParticipation();
    }



    public TRequired InstantiateQuery<TActual, TRequired>()
        where TActual : notnull, ICanRead, TRequired
    {
        // If in a test, check if there's a mock first.
        if (isTesting)
        {
            if (mockContainer!.Get<TActual, TRequired>(out var mock))
            {
                return mock!;
            }
        }

        var query = _serviceProvider.GetRequiredService<TActual>();

        if (query is AddressableChild executor)
        {
            executor.SetGrainId(_componentId, _grainId);
        }

        if (query is BaseStatefulQueryHandler<T> handler)
        {
            var managerGrainId = _managerGrainId;
            handler.SetFactories(
                _grainFactory,
                _queryFactory
            );

            (handler as ISetGetState<T>).SetGetState(this);
            (handler as ISetManagerGrain).SetManagerGrain(managerGrainId);
        }
        else
        {
            throw new InvalidOperationException("Cannot execute a query that does not inherit from BaseStatefulQueryHandler from a TransactionChildGrain.");
        }

        return query;
    }

    public TRequired InstantiateCommand<TActual, TRequired>()
        where TActual : notnull, ICanReadAndWrite, TRequired
    {
        // If in a test, check if there's a mock first.
        if (isTesting)
        {
            if (mockContainer!.Get<TActual, TRequired>(out var mock))
            {
                return mock!;
            }
        }

        var command = _serviceProvider.GetRequiredService<TActual>();

        if (command is AddressableChild executor)
        {
            executor.SetGrainId(_componentId, _grainId);
        }

        if (command is BaseStatefulCommandHandler<T> handler)
        {
            var managerGrainId = _managerGrainId;

            handler.SetFactories(
                _grainFactory,
                _commandFactory,
                _queryFactory
            );

            (handler as ISetGetState<T>).SetGetState(this);
            (handler as ISetUpdateState<T>).SetUpdateState(this);
            (handler as ISetManagerGrain).SetManagerGrain(managerGrainId);
        }
        else
        {
            throw new InvalidOperationException("Cannot execute a command that does not inherit from BaseStatefulCommandHandler from a TransactionChildGrain.");
        }

        return command;
    }

    public TMapper InstantiateMapper<TMapper>() where TMapper : notnull
    {
        return _serviceProvider.GetRequiredService<TMapper>();
    }

    public TMerger InstantiateMerger<TMerger>() where TMerger : notnull
    {
        return _serviceProvider.GetRequiredService<TMerger>();
    }

    public TReducer InstantiateReducer<TReducer>() where TReducer : notnull
    {
        return _serviceProvider.GetRequiredService<TReducer>();
    }
}
