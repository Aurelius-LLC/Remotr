namespace Remotr;

public interface IRunBuilder<T, K> : IHaveGrainFactory, IResolveChildGrain<T>
    where T : IGrain
    where K : IAsyncResult
{
    internal K RunManagerGrain<X>(X grain) where X : T, ITransactionManagerGrain;
    internal K RunChildGrain<X, TState>(X grain, bool interleave)
        where X : T, ITransactionChildGrain<TState>
        where TState : new();
}
