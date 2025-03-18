namespace Remotr;

public abstract class EntityQueryHandler<TState, Output> : BaseEntityQueryHandler<TState>, IAsyncQueryHandler<IAggregateEntity<TState>, Output>
    where TState : new()
{
    public abstract Task<Output> Execute();
}

public abstract class EntityQueryHandler<TState, Input, Output> : BaseEntityQueryHandler<TState>, IAsyncQueryHandler<IAggregateEntity<TState>, Input, Output>
    where TState : new()
{
    public abstract Task<Output> Execute(Input input);

    public static EntityQueryHandler<TState, Input, Output> Q => default;
}