namespace Remotr;

public abstract class BaseStatefulCommandHandler<TState> : BaseStatefulQueryHandler<TState>, ISetUpdateState<TState>
    where TState : new()
{

    private ICanUpdateState<TState> Updater { get; set; } = default!;

    public IInternalCommandFactory CommandFactory { get; private set; } = default!;

    internal void SetFactories(IGrainFactory grainFactory, IInternalCommandFactory commandFactory, IInternalQueryFactory queryFactory)
    {
        CommandFactory = commandFactory;
        SetFactories(grainFactory, queryFactory);
    }

    void ISetUpdateState<TState>.SetUpdateState(ICanUpdateState<TState> updater)
    {
        Updater = updater;
    }

    public ValueTask UpdateState(TState newState)
    {
        return Updater.UpdateState(newState);
    }
}
