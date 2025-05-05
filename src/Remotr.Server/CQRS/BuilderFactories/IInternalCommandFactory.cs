namespace Remotr;

public interface IInternalCommandFactory
{
    public IGrainCommandBaseBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>> GetEntity<T>()
            where T : new();

    public IGrainQueryBaseBuilder<T, BaseRootQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}