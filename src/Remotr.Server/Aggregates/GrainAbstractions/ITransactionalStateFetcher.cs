using Orleans.Concurrency;

namespace Remotr;

public interface ITransactionalStateFetcher : IGrain
{
    [AlwaysInterleave]
    Task<Immutable<T>> GetState<T>(string itemId) where T : new();
}
