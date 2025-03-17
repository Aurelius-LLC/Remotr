namespace Remotr;

public abstract class StatelessQueryHandler<IAggregate, Output> : BaseStatelessQueryHandler<IAggregate>, IAsyncQueryHandler<IAggregate, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute();
}

public abstract class StatelessQueryHandler<IAggregate, Input, Output> : BaseStatelessQueryHandler<IAggregate>, IAsyncQueryHandler<IAggregate, Input, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute(Input input);
}