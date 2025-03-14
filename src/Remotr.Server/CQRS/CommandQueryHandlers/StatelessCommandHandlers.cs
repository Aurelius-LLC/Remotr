namespace Remotr;

public abstract class StatelessCommandHandler<IManagerGrain> : BaseStatelessCommandHandler<IManagerGrain>, IAsyncCommandHandler<IManagerGrain>
    where IManagerGrain : ITransactionManagerGrain
{
    public abstract Task Execute();
}

public abstract class StatelessCommandHandler<IManagerGrain, Output> : BaseStatelessCommandHandler<IManagerGrain>, IAsyncCommandHandler<IManagerGrain, Output>
    where IManagerGrain : ITransactionManagerGrain
{
    public abstract Task<Output> Execute();
}

public abstract class StatelessCommandHandler<IManagerGrain, Input, Output> : BaseStatelessCommandHandler<IManagerGrain>, IAsyncCommandHandler<IManagerGrain, Input, Output>
    where IManagerGrain : ITransactionManagerGrain
{
    public abstract Task<Output> Execute(Input input);
}