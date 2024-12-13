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
            return cachedItem!.CachedItemForRead!;
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

    public async ValueTask<T> GetState<T>(string itemId, Guid transactionId) where T : new()
    {
        // Check if item is in cache and get it if it is.
        var isItemCached = ItemCache.TryGetValue(itemId, out var cachedItem);

        // Return item if in cache and (was not updated or only updated within this transaction).
        if (isItemCached && cachedItem!.CachedItemForTransaction != null && cachedItem!.CachedItemForTransaction.TransactionId == transactionId)
        {
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
        // Remove from cache if it exists.
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
