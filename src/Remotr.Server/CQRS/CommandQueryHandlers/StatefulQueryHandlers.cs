namespace Remotr;

public abstract class StatefulQueryHandler<TState, Output> : BaseStatefulQueryHandler<TState>, IAsyncQueryHandler<IAggregateEntity<TState>, Output>
    where TState : new()
{
    public abstract Task<Output> Execute();
}

public abstract class StatefulQueryHandler<TState, Input, Output> : BaseStatefulQueryHandler<TState>, IAsyncQueryHandler<IAggregateEntity<TState>, Input, Output>
    where TState : new()
{
    public abstract Task<Output> Execute(Input input);

    public static StatefulQueryHandler<TState, Input, Output> Q => default;
}