using System.Reflection;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Remotr;

public static class UseRemotrExtension
{
    private static bool _useRemotrCalled = false;

    public static ISiloBuilder UseRemotr(
        this ISiloBuilder builder,
        Action<RemotrRequestExecutorBuilder> action
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (_useRemotrCalled)
        {
            throw new InvalidOperationException("UseRemotr has already been called.");
        }
        _useRemotrCalled = true;
        UseRemotrHelper(builder);
        RegisterCQs(builder.Services);

        action(new RemotrRequestExecutorBuilder(builder));

        return builder;
    }

    internal static void RegisterCQs(this IServiceCollection services)
    {
        List<Type> assemblyTypes = new();

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            assemblyTypes.AddRange(a.GetTypes());
        }

        var cqs = assemblyTypes.Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IDiscoverableCq)) == typeof(IDiscoverableCq));

        foreach (var cq in cqs)
        {
            services.Add(new ServiceDescriptor(cq, cq, ServiceLifetime.Transient));
        }
    }

    internal static void UseRemotrHelper(ISiloBuilder builder)
    {
        builder.Services.AddSingleton<IUtcDateService, UtcDateService>();
    }
}

public sealed class RemotrRequestExecutorBuilder
{
    private static bool _isConstructed = false;

    private readonly ISiloBuilder _builder;

    private UseCosmosPersistenceBuilder? _cosmosPersistenceBuilder;

    public IServiceCollection Services => _builder.Services;

    internal RemotrRequestExecutorBuilder(ISiloBuilder builder)
    {
        if (_isConstructed)
        {
            throw new InvalidOperationException("RemotrRequestExecutorBuilder has already been constructed.");
        }
        _isConstructed = true;
        _builder = builder;
    }

    public RemotrRequestExecutorBuilder UseCosmosPersistence(CosmosClient client, string databaseId, Action<UseCosmosPersistenceBuilder> action)
    {
        if (_cosmosPersistenceBuilder is not null)
        {
            throw new InvalidOperationException("Cosmos persistence has already been configured.");
        }
        _cosmosPersistenceBuilder = new UseCosmosPersistenceBuilder(_builder.Services, client, databaseId);
        action(_cosmosPersistenceBuilder);
        return this;
    }
}

public sealed class UseCosmosPersistenceBuilder
{
    private static bool _isConstructed = false;

    private readonly IServiceCollection _services;

    private readonly CosmosClient _client;

    private readonly string _databaseId;

    private readonly HashSet<string> _containerIds = new();

    internal static JsonSerializerOptions? SerializerOptions { get; private set; }

    public static JsonSerializerOptions? GetSerializerOptions() => SerializerOptions;

    internal UseCosmosPersistenceBuilder(IServiceCollection services, CosmosClient client, string databaseId)
    {
        if (_isConstructed)
        {
            throw new InvalidOperationException("UseCosmosPersistenceBuilder has already been constructed.");
        }
        _isConstructed = true;
        _services = services;
        _client = client;
        _databaseId = databaseId;
    }

    public UseCosmosPersistenceBuilder UseJsonSerializer(JsonSerializerOptions options)
    {
        if (SerializerOptions is not null)
        {
            throw new InvalidOperationException("Json serializer has already been configured.");
        }
        SerializerOptions = options;
        return this;
    }

    public UseCosmosPersistenceBuilder AddContainer(string containerId, string partitionKeyPath, Func<GrainId, string> grainIdToPartitionKey, Func<Container, Task>? initializeContainer = null)
    {
        if (_containerIds.Contains(containerId))
        {
            throw new InvalidOperationException($"Container with id {containerId} has already been added.");
        }
        _containerIds.Add(containerId);
        _services.AddKeyedSingleton<IPersistentStore>(containerId, new CosmosDbPersistentStore(_client, _databaseId, containerId, partitionKeyPath, grainIdToPartitionKey, initializeContainer));
        return this;
    }
}
