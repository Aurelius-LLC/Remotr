namespace Remotr;

public abstract class BaseRootCommandHandler<IAggregate> : BaseRootQueryHandler<IAggregate>
    where IAggregate : IAggregateRoot
{
    public IInternalCommandFactory CommandFactory { get; private set; } = default!;

    internal void SetFactories(IGrainFactory grainFactory, IInternalCommandFactory commandFactory, IInternalQueryFactory queryFactory)
    {
        CommandFactory = commandFactory;
        SetFactories(grainFactory, queryFactory);
    }
}