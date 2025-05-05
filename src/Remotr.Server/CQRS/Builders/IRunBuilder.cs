namespace Remotr;

public interface IRunBuilder<T, K> : IHaveGrainFactory, IResolveEntityGrain<T>
    where T : IGrain
    where K : IAsyncResult
{
    internal K RunAggregate<X>(X grain) where X : T, IAggregateRoot;
    internal K RunEntityGrain<X, TState>(X grain, bool interleave)
        where X : T, IAggregateEntity<TState>
        where TState : new();
}
