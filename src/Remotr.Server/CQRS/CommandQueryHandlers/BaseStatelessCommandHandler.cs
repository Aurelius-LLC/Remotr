namespace Remotr;

public abstract class BaseStatelessCommandHandler<IAggregate> : BaseStatelessQueryHandler<IAggregate>
    where IAggregate : IAggregateRoot
{
    public IInternalCommandFactory CommandFactory { get; private set; } = default!;

    internal void SetFactories(IGrainFactory grainFactory, IInternalCommandFactory commandFactory, IInternalQueryFactory queryFactory)
    {
        CommandFactory = commandFactory;
        SetFactories(grainFactory, queryFactory);
    }
}