namespace Remotr;

public abstract class EntityCommandHandler<TState> : BaseEntityCommandHandler<TState>, IAsyncCommandHandler<IAggregateEntity<TState>>
    where TState : new()
{
    public abstract Task Execute();
}

public abstract class EntityCommandHandler<TState, Output> : BaseEntityCommandHandler<TState>, IAsyncCommandHandler<IAggregateEntity<TState>, Output>
    where TState : new()
{
    public abstract Task<Output> Execute();
}

public abstract class EntityCommandHandler<TState, Input, Output> : BaseEntityCommandHandler<TState>, IAsyncCommandHandler<IAggregateEntity<TState>, Input, Output>
    where TState : new()
{
    public abstract Task<Output> Execute(Input input);
}