namespace Remotr;

public interface IExternalCommandFactory
{
    public IGrainCommandBaseBuilder<T, BaseStatelessCommandHandler<T>, BaseStatelessQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}

