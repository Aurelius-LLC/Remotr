using System.Text.Json;
using Orleans.Runtime;

namespace Remotr;

public class InternalQueryFactory : IInternalQueryFactory
{
    private readonly IGrainFactory _grainFactory;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly GrainId _managerId;

    internal InternalQueryFactory(IGrainFactory grainFactory, JsonSerializerOptions serializerOptions, GrainId managerId)
    {
        _grainFactory = grainFactory;
        _serializerOptions = serializerOptions;
        _managerId = managerId;
    }

    public IGrainQueryBaseBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>> GetChild<T>()
        where T : new()
    {
        UniversalBuilder<ITransactionChildGrain<T>, object> builder = new(new EmptyStep());
        return new GrainQueryBaseBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>>(
                _grainFactory,
                (string key) =>
                {
                    var componentId = new ComponentId
                    {
                        ManagerGrainId = _managerId,
                        ItemId = key
                    };
                    return _grainFactory.GetGrain<ITransactionChildGrain<T>>(JsonSerializer.Serialize(componentId, _serializerOptions));
                },
                builder
        );
    }

    public IGrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>> GetManager<T>() where T : ITransactionManagerGrain
    {
        UniversalBuilder<T, object> builder = new(new EmptyStep());
        return new GrainQueryBaseBuilder<T, BaseStatelessQueryHandler<T>>(
            _grainFactory,
            (string key) => throw new InvalidOperationException("Cannot resolve child grain from manager grain"),
            builder
        );
    }
}
