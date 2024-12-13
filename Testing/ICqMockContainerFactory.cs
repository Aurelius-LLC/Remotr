namespace Remotr.Testing;

public interface ICqMockContainerFactory
{
    internal ICqMockContainer GetContainer(Guid testId);
}

