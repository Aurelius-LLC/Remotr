namespace Remotr;

public abstract class StatefulCommandHandler<TState> : BaseStatefulCommandHandler<TState>, IAsyncCommandHandler<ITransactionChildGrain<TState>>
    where TState : new()
{
    public abstract Task Execute();
}

public abstract class StatefulCommandHandler<TState, Output> : BaseStatefulCommandHandler<TState>, IAsyncCommandHandler<ITransactionChildGrain<TState>, Output>
    where TState : new()
{
    public abstract Task<Output> Execute();
}

public abstract class StatefulCommandHandler<TState, Input, Output> : BaseStatefulCommandHandler<TState>, IAsyncCommandHandler<ITransactionChildGrain<TState>, Input, Output>
    where TState : new()
{
    public abstract Task<Output> Execute(Input input);
}