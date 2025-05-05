namespace Remotr;

public interface IExternalCommandFactory
{
    public IGrainCommandBaseBuilder<T, BaseRootCommandHandler<T>, BaseRootQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}

