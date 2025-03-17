namespace Remotr;

public abstract class StatefulCommandHandler<TState> : BaseStatefulCommandHandler<TState>, IAsyncCommandHandler<IAggregateEntity<TState>>
    where TState : new()
{
    public abstract Task Execute();
}

public abstract class StatefulCommandHandler<TState, Output> : BaseStatefulCommandHandler<TState>, IAsyncCommandHandler<IAggregateEntity<TState>, Output>
    where TState : new()
{
    public abstract Task<Output> Execute();
}

public abstract class StatefulCommandHandler<TState, Input, Output> : BaseStatefulCommandHandler<TState>, IAsyncCommandHandler<IAggregateEntity<TState>, Input, Output>
    where TState : new()
{
    public abstract Task<Output> Execute(Input input);
}