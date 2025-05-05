namespace Remotr;

public abstract class RootQueryHandler<IAggregate, Output> : BaseRootQueryHandler<IAggregate>, IAsyncQueryHandler<IAggregate, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute();
}

public abstract class RootQueryHandler<IAggregate, Input, Output> : BaseRootQueryHandler<IAggregate>, IAsyncQueryHandler<IAggregate, Input, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute(Input input);
}