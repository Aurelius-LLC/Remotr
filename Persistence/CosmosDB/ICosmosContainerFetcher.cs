using Microsoft.Azure.Cosmos;

namespace Remotr;

public interface ICosmosContainerFetcher
{
    public ValueTask<Container> GetContainer(string containerName);
}
