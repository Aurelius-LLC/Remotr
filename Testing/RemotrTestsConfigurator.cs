using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Remotr.Testing;


public class RemotrTestsConfigurator
{
    private readonly TestPersistentStore persistentStore;
    private readonly CqMockContainerFactory cqMockContainerFactory;


    private static RemotrTestsConfigurator _instance;
    public static RemotrTestsConfigurator Instance
    {
        get
        {
            _instance ??= new RemotrTestsConfigurator();
            return _instance;
        }
    }

    private RemotrTestsConfigurator()
    {
        persistentStore = new TestPersistentStore();
        cqMockContainerFactory = new CqMockContainerFactory();
    }

    public void ConfigureSiloServices(ISiloBuilder builder)
    {
        var webSerializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        webSerializationOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        builder.Services
            .AddSingleton<IJsonSerializerWrapper>(new JsonSerializerWrapper(webSerializationOptions))
            .AddSingleton(jsonSerializerOptions)
            .AddSingleton<IUtcDateService, UtcDateService>()
            .AddSingleton(persistentStore)
            .AddKeyedSingleton<IPersistentStore>(KeyedService.AnyKey, persistentStore)
            .AddSingleton(cqMockContainerFactory)
            .AddSingleton<ICqMockContainerFactory>(cqMockContainerFactory)
            .AddSingleton<ITestRunner, TestRunner>();

        UseRemotrExtension.RegisterCQs(builder.Services);
    }

    public IServiceProvider GetServiceProvider(IGrainFactory grainFactory)
    {
        var testRunner = new TestRunner(
            grainFactory,
            persistentStore,
            cqMockContainerFactory
        );

        return new ServiceCollection()
            .AddSingleton<IUtcDateService, UtcDateService>()
            .AddSingleton(persistentStore)
            .AddKeyedSingleton<IPersistentStore>(KeyedService.AnyKey, persistentStore)
            .AddSingleton(cqMockContainerFactory)
            .AddSingleton<ICqMockContainerFactory>(cqMockContainerFactory)
            .AddSingleton<ITestRunner>(testRunner)
            .BuildServiceProvider();
    }
}