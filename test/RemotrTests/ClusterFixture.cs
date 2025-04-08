using Orleans.TestingHost;
using Remotr.Testing;

namespace RemotrTests.Tests;

public class ClusterFixture : IDisposable
{
    public ClusterFixture()
    {

        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<SiloConfigurator>();
        Cluster = builder.Build();
        Cluster.Deploy();

        Services = RemotrTestsConfigurator.Instance.GetServiceProvider(Cluster.GrainFactory);
    }

    public void Dispose()
    {
        Cluster.StopAllSilos();
    }

    public TestCluster Cluster { get; private set; }
    public IServiceProvider Services { get; private set; }

    private class SiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            RemotrTestsConfigurator.Instance.ConfigureSiloServices(siloBuilder);
            siloBuilder.AddMemoryGrainStorageAsDefault();
        }
    }
}