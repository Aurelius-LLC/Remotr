namespace Remotr;

public interface IInternalQueryFactory
{
    public IGrainQueryBaseBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>> GetChild<T>()
        where T : new();

    public IGrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>> GetManager<T>() where T : ITransactionManagerGrain;
}

