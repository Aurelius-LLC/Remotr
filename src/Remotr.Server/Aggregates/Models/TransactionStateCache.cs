using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Serialization;

namespace Remotr;

[GenerateSerializer]
internal sealed class TransactionStateCache
{

    private readonly DeepCopier _deepCopier;

    private IDictionary<string, CachedItem> ItemCache { get; set; } = new Dictionary<string, CachedItem>();

    private Func<ITransactionalStateFetcher> GetFetcherFunc { get; set; }

    private JsonSerializerOptions _jsonSerializerOptions;

    // Track the current fetch operation and its timestamp
    private Task<object?>? _currentFetchTask;

    public TransactionStateCache(JsonSerializerOptions jsonSerializerOptions, Func<ITransactionalStateFetcher> getFetcherFunc, IServiceProvider serviceProvider)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        GetFetcherFunc = getFetcherFunc;
        _deepCopier = serviceProvider.GetService<DeepCopier>()!;
    }

    public List<TransactionStateOperation> GetOperations(Guid transactionId)
    {
        List<TransactionStateOperation> dataOperations = new();

        // Create operations for items that were modified during the transaction.
        foreach (var (itemId, cachedItem) in ItemCache)
        {
            if (cachedItem.CachedItemForTransaction != null && cachedItem.CachedItemForTransaction.TransactionId == transactionId)
            {
                // If is deleted, create delete operation.
                // Else, create upsert operation.
                if (cachedItem.CachedItemForTransaction.IsDeleted)
                {
                    dataOperations.Add(new()
                    {
                        ItemId = itemId,
                        TransactionOperationType = TransactionStateOperationType.Delete,
                    });
                }
                else
                {
                    dataOperations.Add(new()
                    {
                        ItemId = itemId,
                        TransactionOperationType = TransactionStateOperationType.Upsert,
                        ItemJson = JsonSerializer.Serialize(cachedItem.CachedItemForTransaction.Item, _jsonSerializerOptions)
                    });
                }
            }
        }
        return dataOperations;
    }

    public async ValueTask<T> GetReadOnlyState<T>(string itemId, DateTime beforeTimestamp) where T : new()
    {
        // Check if item is in cache and get it if it is.
        var isItemCached = ItemCache.TryGetValue(itemId, out var cachedItem);

        // If the state has a finished transaction, then we need to refresh the local state.
        var stateHasFinishedTransaction = isItemCached && cachedItem!.CachedItemForTransaction != null && cachedItem.CachedItemForTransaction!.TransactionTimestamp < beforeTimestamp;

        if (!stateHasFinishedTransaction && isItemCached && cachedItem!.CachedItemForRead != null)
        {
            return _deepCopier.Copy(cachedItem!.CachedItemForRead!);
        }

        // Check if there's already a fetch in progress for this timestamp
        if (_currentFetchTask != null)
        {
            // Wait for the existing fetch to complete
            await _currentFetchTask;
            // The result should now be in the cache
            return _deepCopier.Copy(ItemCache[itemId].CachedItemForRead!);
        }

        // Start a new fetch operation
        var fetchTask = GetFetcherFunc().GetState<T>(itemId);
        _currentFetchTask = fetchTask.ContinueWith(t => (object?)t.Result);

        try
        {
            // Get item and cache it
            var item = await fetchTask;
            ItemCache[itemId] = new()
            {
                CachedItemForRead = item,
            };

            // Always copy and then return the state to keep it safe from modifications.
            return _deepCopier.Copy(item);
        }
        finally
        {
            // Clear the fetch task reference once complete
            _currentFetchTask = null;
        }
    }

    public async ValueTask<T> GetState<T>(string itemId, Guid transactionId) where T : new()
    {
        // Check if item is in cache and get it if it is.
        var isItemCached = ItemCache.TryGetValue(itemId, out var cachedItem);

        // Return item if in cache and (was not updated or only updated within this transaction).
        if (isItemCached && cachedItem!.CachedItemForTransaction != null && cachedItem!.CachedItemForTransaction.TransactionId == transactionId)
        {
            // Check if the item was marked as deleted in this transaction
            if (cachedItem.CachedItemForTransaction.IsDeleted)
            {
                // If deleted, return a new instance
                return new T();
            }
            
            return _deepCopier.Copy(cachedItem!.CachedItemForTransaction!.Item);
        }

        // Reset the cached item for transaction if it was set before since it has a different transaction id.
        if (isItemCached && cachedItem!.CachedItemForTransaction != null)
        {
            cachedItem!.CachedItemForTransaction = null;
        }
        // If the item is in the cache but has never been updated, return the cached item.
        else if (isItemCached && cachedItem!.CachedItemForTransaction == null)
        {
            return _deepCopier.Copy(cachedItem!.CachedItemForRead);
        }

        // Get item and cache it.
        var item = await GetFetcherFunc().GetState<T>(itemId);
        ItemCache[itemId] = new()
        {
            CachedItemForRead = item,
        };

        // Always copy and then return the state to keep it safe from modifications.
        return _deepCopier.Copy(item);
    }

    public void UpdateState<T>(string itemId, T item, Guid transactionId, DateTime transactionTimestamp) where T : new()
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (!ItemCache.ContainsKey(itemId))
        {
            ItemCache[itemId] = new()
            {
                CachedItemForTransaction = new()
                {
                    TransactionTimestamp = transactionTimestamp,
                    TransactionId = transactionId,
                    Item = item,
                    IsDeleted = false,
                }
            };
        }

        ItemCache[itemId] = ItemCache[itemId] with
        {
            CachedItemForTransaction = new()
            {
                TransactionTimestamp = transactionTimestamp,
                TransactionId = transactionId,
                Item = item,
                IsDeleted = false,
            }
        };
    }

    // TODO: fix bug where it won't clear the state if the item hasn't been fetched first.
    public void ClearState(string itemId, Guid transactionId, DateTime transactionTimestamp)
    {
        // Set the item to be deleted in the cache if it's there already.
        if (ItemCache.TryGetValue(itemId, out CachedItem? value))
        {
            ItemCache[itemId] = value with
            {
                CachedItemForTransaction = new()
                {
                    TransactionTimestamp = transactionTimestamp,
                    TransactionId = transactionId,
                    IsDeleted = true,
                }
            };
        }
        else
        {
            ItemCache[itemId] = new()
            {
                CachedItemForTransaction = new()
                {
                    TransactionTimestamp = transactionTimestamp,
                    TransactionId = transactionId,
                    IsDeleted = true,
                }
            };
        }
    }
}
