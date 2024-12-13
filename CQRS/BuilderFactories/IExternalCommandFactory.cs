namespace Remotr;

public interface IExternalCommandFactory
{
    public IGrainCommandBaseBuilder<T, BaseStatelessCommandHandler<T>, BaseStatelessQueryHandler<T>> GetManager<T>() where T : ITransactionManagerGrain;
}

