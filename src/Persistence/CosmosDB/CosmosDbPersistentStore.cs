using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Orleans.Runtime;

namespace Remotr;

public sealed record CosmosDbPersistentStore : IPersistentStore
{
    private readonly CosmosClient _client;

    private readonly string _databaseId;

    private bool _initialized = false;

    private Task? _beingInitialized = null;

    private readonly string _containerId;

    private readonly string _partitionKeyPath;

    private readonly Func<GrainId, string> _grainIdToPartitionKey;

    private readonly Func<Container, Task>? _initializeContainer;

    private JsonSerializerOptions? _jsonSerializerOptions;

    private Container? _container;

    private readonly object _initializeLock = new();

    public CosmosDbPersistentStore(CosmosClient client, string databaseId, string containerId, string partitionKeyPath, Func<GrainId, string> grainIdToPartitionKey, Func<Container, Task>? initializeContainer)
    {
        _client = client;
        _databaseId = databaseId;
        _containerId = containerId;
        _partitionKeyPath = partitionKeyPath;
        _grainIdToPartitionKey = grainIdToPartitionKey;
        _initializeContainer = initializeContainer;
    }

    public async Task<T> ReadItem<T>(string itemId, GrainId grainId) where T : new()
    {
        await InitializeContainer();
        var partitionKey = _grainIdToPartitionKey(grainId);

        if (!typeof(T).GetInterfaces().Contains(typeof(ICosmosPartitionedItem)))
        {
            throw new Exception("The item does not implement the interface ICosmosPartitionedItem.");
        }

        // Read item from persistent store. If item does not exist, return a new T with the partition key set.
        try
        {
            var response = await _container!.ReadItemStreamAsync(itemId, new(partitionKey));
            if (response.Content is null)
            {
                var newT = new T();
                if (newT is ICosmosPartitionedItem item)
                {
                    item.SetPartitionKey(partitionKey.ToString());
                }
                return newT;
            }
            var itemJson = await new StreamReader(response.Content).ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(itemJson, _jsonSerializerOptions)!;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            var newT = new T();
            if (newT is ICosmosPartitionedItem item)
            {
                item.SetPartitionKey(partitionKey.ToString());
            }
            return newT;
        }
    }

    public async Task<IDictionary<string, string>> ExecuteTransaction(IEnumerable<TransactionStateOperation> transactionOperations, GrainId grainId)
    {
        await InitializeContainer();
        var partitionKey = _grainIdToPartitionKey(grainId);

        // Create transactional batch.
        var transactionalBatch = _container!.CreateTransactionalBatch(new(partitionKey));

        // Add operations to transactional batch.
        foreach (var transactionOperation in transactionOperations)
        {
            var type = transactionOperation.TransactionOperationType;
            var itemId = transactionOperation.ItemId;
            var itemJson = transactionOperation.ItemJson;
            switch (type)
            {
                case TransactionStateOperationType.Upsert:
                    var item = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(itemJson!)!;
                    item["id"] = itemId!;
                    item[_partitionKeyPath.Replace("/", "")] = partitionKey;
                    itemJson = JsonSerializer.Serialize(item);
                    var bytes = Encoding.UTF8.GetBytes(itemJson);
                    var stream = new MemoryStream(bytes);
                    transactionalBatch.UpsertItemStream(stream);
                    break;

                case TransactionStateOperationType.Delete:
                    transactionalBatch.DeleteItem(itemId!);
                    break;
            }
        }

        // Execute transactional batch.
        var responses = await transactionalBatch.ExecuteAsync();

        // If transaction failed, throw exception.
        if (!responses.IsSuccessStatusCode)
        {
            throw new Exception(responses.ErrorMessage);
        }

        // Get dictionary mapping item ids to the new state of the updated item.
        var updatedItems = new Dictionary<string, string>();
        var itemIds = transactionOperations.Select(transactionOperation => transactionOperation.ItemId).ToList();
        var transactionOperationTypes = transactionOperations.Select(transactionOperation => transactionOperation.TransactionOperationType).ToList();

        List<(string itemId, Task<string> result)> readingItems = new();
        for (var i = 0; i < itemIds.Count; i++)
        {
            // Ignore deleted items.
            if (transactionOperationTypes[i] == TransactionStateOperationType.Delete)
            {
                continue;
            }
            var itemId = itemIds[i];
            if (responses[i].ResourceStream is not null)
            {
                readingItems.Add((itemId!, new StreamReader(responses[i].ResourceStream, Encoding.UTF8).ReadToEndAsync()));
            }
        }

        await Task.WhenAll(readingItems.Select(item => item.result));
        foreach (var item in readingItems)
        {
            updatedItems[item.itemId] = await item.result;
        }

        return updatedItems;
    }

    private async Task MakeInitializeCalls()
    {
        _jsonSerializerOptions = UseCosmosPersistenceBuilder.SerializerOptions;
        var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_databaseId);
        var database = databaseResponse.Database;
        var containerResponse = await database.CreateContainerIfNotExistsAsync(
            new ContainerProperties
            {
                Id = _containerId,
                PartitionKeyPath = _partitionKeyPath
            });
        _container = containerResponse.Container;
        if (_initializeContainer is not null)
        {
            await _initializeContainer(_container);
        }
        _initialized = true;
    }

    private async ValueTask InitializeContainer()
    {
        if (_initialized)
        {
            return;
        }
        else if (_beingInitialized is not null)
        {
            await _beingInitialized;
            if (_initialized)
            {
                return;
            }
        }

        lock (_initializeLock)
        {
            if (_beingInitialized is not null && _beingInitialized.IsCompleted && !_initialized)
            {
                _beingInitialized = null;
            }
            if (_beingInitialized is null)
            {
                _beingInitialized = MakeInitializeCalls();
            }
        }

        await _beingInitialized;
        if (!_initialized)
        {
            throw new Exception("Failed to initialize CosmosDB container.");
        }
    }
}
