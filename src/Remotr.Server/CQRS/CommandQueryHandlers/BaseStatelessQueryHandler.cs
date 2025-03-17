using Orleans.Runtime;

namespace Remotr;

public abstract class BaseStatelessQueryHandler<IAggregate> : AddressableManager, IDiscoverableCq
    where IAggregate : IAggregateRoot
{
    public GrainId GrainId { get; private set; } = default!;

    private IAddressable _address { get; set; } = default!;
    public override IAddressable Address
    {
        get => _address;
    }

    public IGrainFactory GrainFactory { get; private set; } = default!;

    public IInternalQueryFactory QueryFactory { get; private set; } = default!;

    internal void SetFactories(IGrainFactory grainFactory, IInternalQueryFactory queryFactory)
    {
        GrainFactory = grainFactory;
        QueryFactory = queryFactory;
    }

    internal override void SetGrainId(IAddressable addressable, GrainId grainId)
    {
        _address = addressable;
        GrainId = grainId;
    }
}
