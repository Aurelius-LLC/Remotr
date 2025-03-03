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

    public IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> GetChild<T>()
        where T : new()
    {
        UniversalBuilder<ITransactionChildGrain<T>, object> builder = new(new EmptyStep());
        return new GrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>>(
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
}
