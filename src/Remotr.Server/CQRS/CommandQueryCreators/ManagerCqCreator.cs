using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Remotr.Testing;

namespace Remotr;

public class ManagerCqCreator<IAggregate> : ICqCreator
    where IAggregate : IAggregateRoot
{
    public bool isTesting = false;
    public ICqMockContainer? mockContainer;

    private readonly IServiceProvider _serviceProvider;
    private readonly IAddressable _addressable;
    private readonly GrainId _grainId;
    private readonly IGrainFactory _grainFactory;
    private readonly IInternalQueryFactory _queryFactory;
    private readonly IInternalCommandFactory _commandFactory;

    internal ManagerCqCreator(
        IServiceProvider serviceProvider,
        IAddressable addressable,
        GrainId grainId,
        IGrainFactory grainFactory,
        IInternalQueryFactory queryFactory,
        IInternalCommandFactory commandFactory
    )
    {
        _serviceProvider = serviceProvider;
        _addressable = addressable;
        _grainId = grainId;
        _grainFactory = grainFactory;
        _queryFactory = queryFactory;
        _commandFactory = commandFactory;
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

        if (query is AddressableManager executor)
        {
            executor.SetGrainId(_addressable, _grainId);
        }

        if (query is BaseStatelessQueryHandler<IAggregate> handler)
        {
            handler.SetFactories(
                _grainFactory,
                _queryFactory
            );
        }
        else
        {
            throw new InvalidOperationException("Cannot execute a query that does not inherit from BaseStatefulQueryHandler from a AggregateEntity.");
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

        if (command is AddressableManager executor)
        {
            executor.SetGrainId(_addressable, _grainId);
        }

        if (command is BaseStatelessCommandHandler<IAggregate> handler)
        {
            handler.SetFactories(
                _grainFactory,
                _commandFactory,
                _queryFactory
            );
        }
        else
        {
            throw new InvalidOperationException("Cannot execute a command that does not inherit from BaseStatefulCommandHandler from a AggregateEntity.");
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
