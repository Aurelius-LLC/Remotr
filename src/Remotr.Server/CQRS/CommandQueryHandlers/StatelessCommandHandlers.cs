namespace Remotr;

public abstract class RootCommandHandler<IAggregate> : BaseRootCommandHandler<IAggregate>, IAsyncCommandHandler<IAggregate>
    where IAggregate : IAggregateRoot
{
    public abstract Task Execute();
}

public abstract class RootCommandHandler<IAggregate, Output> : BaseRootCommandHandler<IAggregate>, IAsyncCommandHandler<IAggregate, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute();
}

public abstract class RootCommandHandler<IAggregate, Input, Output> : BaseRootCommandHandler<IAggregate>, IAsyncCommandHandler<IAggregate, Input, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute(Input input);
}