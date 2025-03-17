using Remotr;

namespace Trackr.Backend.Core.Remotr.CQRS.BuilderFactories;

public class ExternalQueryFactory : IExternalQueryFactory
{
    private readonly IGrainFactory _grainFactory;

    public ExternalQueryFactory(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public IGrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>> GetAggregate<T>() where T : IAggregateRoot
    {
        UniversalBuilder<T, object> builder = new(new EmptyStep());
        return new GrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>>(
            _grainFactory,
            (string key) => throw new InvalidOperationException("Cannot resolve child grain from manager grain"),
            builder
        );
    }
}