using Orleans.Concurrency;

namespace Remotr;

public interface ITransactionalStateFetcher : IGrain
{
    [AlwaysInterleave]
    Task<T> GetState<T>(string itemId) where T : new();
}
