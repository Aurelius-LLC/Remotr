namespace Remotr;

public class ExternalCommandFactory : IExternalCommandFactory
{
    private readonly IGrainFactory _grainFactory;

    public ExternalCommandFactory(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public IGrainCommandBaseBuilder<T, BaseStatelessCommandHandler<T>, BaseStatelessQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot
    {
        UniversalBuilder<T, object> builder = new(new EmptyStep());
        return new GrainCommandBaseBuilder<T, BaseStatelessCommandHandler<T>, BaseStatelessQueryHandler<T>>(
                _grainFactory,
                (string key) => throw new InvalidOperationException("Cannot resolve child grain from manager grain"),
                builder
        );
    }
}