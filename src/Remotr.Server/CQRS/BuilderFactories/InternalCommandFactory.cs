using System.Text.Json;
using Orleans.Runtime;

namespace Remotr;

public class InternalCommandFactory : IInternalCommandFactory
{
    private readonly IGrainFactory _grainFactory;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly GrainId _managerId;

    internal InternalCommandFactory(IGrainFactory grainFactory, JsonSerializerOptions options, GrainId managerId)
    {
        _grainFactory = grainFactory;
        _serializerOptions = options;
        _managerId = managerId;
    }

    public IGrainCommandBaseBuilder<IAggregateEntity<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> GetEntity<T>()
        where T : new()
    {
        UniversalBuilder<IAggregateEntity<T>, object> builder = new(new EmptyStep());
        return new GrainCommandBaseBuilder<IAggregateEntity<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>>(
                _grainFactory,
                (string key) =>
                {
                    var componentId = new ComponentId
                    {
                        AggregateId = _managerId,
                        ItemId = key
                    };
                    return _grainFactory.GetGrain<IAggregateEntity<T>>(JsonSerializer.Serialize(componentId, _serializerOptions));
                },
                builder
        );
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
