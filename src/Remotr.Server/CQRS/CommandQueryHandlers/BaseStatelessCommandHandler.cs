namespace Remotr;

public abstract class BaseStatelessCommandHandler<IManagerGrain> : BaseStatelessQueryHandler<IManagerGrain>
    where IManagerGrain : ITransactionManagerGrain
{
    public IInternalCommandFactory CommandFactory { get; private set; } = default!;

    internal void SetFactories(IGrainFactory grainFactory, IInternalCommandFactory commandFactory, IInternalQueryFactory queryFactory)
    {
        CommandFactory = commandFactory;
        SetFactories(grainFactory, queryFactory);
    }
}