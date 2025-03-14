using Orleans.Runtime;

namespace Remotr;

public interface IPersistentStore
{

    Task<T> ReadItem<T>(string itemId, GrainId grainId) where T : new();

    // Returns a dictionary mapping item ids to the new state of the item.
    Task<IDictionary<string, string>> ExecuteTransaction(IEnumerable<TransactionStateOperation> transactionOperations, GrainId grainId);
}
