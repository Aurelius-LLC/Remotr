namespace Remotr;

public interface IExternalQueryFactory
{
    public IGrainQueryBaseBuilder<T, BaseRootQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}

