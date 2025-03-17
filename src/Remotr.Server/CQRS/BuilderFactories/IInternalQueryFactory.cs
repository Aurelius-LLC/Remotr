namespace Remotr;

public interface IInternalQueryFactory
{
    public IGrainQueryBaseBuilder<IAggregateEntity<T>, BaseStatefulQueryHandler<T>> GetEntity<T>()
        where T : new();

    public IGrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}

