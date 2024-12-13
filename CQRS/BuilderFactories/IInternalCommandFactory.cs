namespace Remotr;

public interface IInternalCommandFactory
{
    public IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> GetChild<T>()
            where T : new();
}

