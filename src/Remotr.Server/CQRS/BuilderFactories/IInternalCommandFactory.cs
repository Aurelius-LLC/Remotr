namespace Remotr;

public interface IInternalCommandFactory
{
    public IGrainCommandBaseBuilder<IAggregateEntity<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> GetEntity<T>()
            where T : new();

    public IGrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot;
}