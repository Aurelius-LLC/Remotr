namespace Remotr;

public interface IInternalQueryFactory
{
    public IGrainQueryBaseBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>> GetEntity<T>()
        where T : new();

    public IGrainQueryBaseBuilder<T, BaseRootQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}

