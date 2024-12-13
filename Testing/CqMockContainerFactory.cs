using System.Collections.Concurrent;

namespace Remotr.Testing;

public class CqMockContainerFactory : ICqMockContainerFactory
{
    private readonly ConcurrentDictionary<Guid, CqMockContainer> _containers = new();

    public ICqMockContainer GetContainer(Guid testId)
    {
        return _containers.GetOrAdd(testId, (testId) => new CqMockContainer());
    }

    public void SetContainer(Guid testId, CqMockContainer container)
    {
        if (!_containers.TryAdd(testId, container))
        {
            throw new InvalidOperationException("Cannot insert two identical mock containers.");
        }
    }
}

