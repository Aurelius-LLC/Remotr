namespace Remotr;

public abstract class StatelessCommandHandler<IAggregate> : BaseStatelessCommandHandler<IAggregate>, IAsyncCommandHandler<IAggregate>
    where IAggregate : IAggregateRoot
{
    public abstract Task Execute();
}

public abstract class StatelessCommandHandler<IAggregate, Output> : BaseStatelessCommandHandler<IAggregate>, IAsyncCommandHandler<IAggregate, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute();
}

public abstract class StatelessCommandHandler<IAggregate, Input, Output> : BaseStatelessCommandHandler<IAggregate>, IAsyncCommandHandler<IAggregate, Input, Output>
    where IAggregate : IAggregateRoot
{
    public abstract Task<Output> Execute(Input input);
}