namespace Remotr;

public interface IExternalQueryFactory
{
    public IGrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>> GetManager<T>() where T : ITransactionManagerGrain;
}

