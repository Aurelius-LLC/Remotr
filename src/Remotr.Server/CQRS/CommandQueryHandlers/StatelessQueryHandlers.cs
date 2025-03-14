namespace Remotr;

public abstract class StatelessQueryHandler<IManagerGrain, Output> : BaseStatelessQueryHandler<IManagerGrain>, IAsyncQueryHandler<IManagerGrain, Output>
    where IManagerGrain : ITransactionManagerGrain
{
    public abstract Task<Output> Execute();
}

public abstract class StatelessQueryHandler<IManagerGrain, Input, Output> : BaseStatelessQueryHandler<IManagerGrain>, IAsyncQueryHandler<IManagerGrain, Input, Output>
    where IManagerGrain : ITransactionManagerGrain
{
    public abstract Task<Output> Execute(Input input);
}