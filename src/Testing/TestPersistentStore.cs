using System.Collections.Concurrent;
using System.Text.Json;
using Orleans.Runtime;

namespace Remotr.Testing;

public sealed class TestPersistentStore : IPersistentStore
{
    private readonly ConcurrentDictionary<GrainId, IDictionary<string, string>> _store = new();

    public Task<IDictionary<string, string>> ExecuteTransaction(IEnumerable<TransactionStateOperation> transactionOperations, GrainId grainId)
    {
        foreach (var operation in transactionOperations)
        {
            if (operation.TransactionOperationType == TransactionStateOperationType.Upsert)
            {
                if (!_store.TryGetValue(grainId, out IDictionary<string, string>? value))
                {
                    value = new Dictionary<string, string>();
                    _store[grainId] = value;
                }
                value[operation.ItemId!] = operation.ItemJson!;
            }
            else if (operation.TransactionOperationType == TransactionStateOperationType.Delete)
            {
                _store[grainId].Remove(operation.ItemId!);
            }
        }
        return Task.FromResult(_store[grainId]);
    }

    public Task<T> ReadItem<T>(string itemId, GrainId grainId) where T : new()
    {

        if (!_store.TryGetValue(grainId, out var items) || !items.TryGetValue(itemId, out var persistedItem))
        {
            var newT = new T();
            return Task.FromResult(newT);
        }
        return Task.FromResult(JsonSerializer.Deserialize<T>(persistedItem))!;
    }

    public void SetJsonState(GrainId grainId, Dictionary<string, string> jsonState)
    {
        _store[grainId] = jsonState;
    }
}
