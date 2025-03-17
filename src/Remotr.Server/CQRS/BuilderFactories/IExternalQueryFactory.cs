namespace Remotr;

public interface IExternalQueryFactory
{
    public IGrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}

