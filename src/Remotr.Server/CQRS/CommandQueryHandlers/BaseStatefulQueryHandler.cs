using Orleans.Runtime;

namespace Remotr;

public abstract class BaseStatefulQueryHandler<TState> : AddressableChild, ISetGetState<TState>, ISetAggregate, IDiscoverableCq
    where TState : new()
{
    public GrainId AggregateId { get; private set; } = default!;
    public GrainId GrainId { get; private set; } = default!;

    private ComponentId _address { get; set; } = default!;
    public override ComponentId Address
    {
        get => _address;
    }

    public IGrainFactory GrainFactory { get; private set; } = default!;

    public IInternalQueryFactory QueryFactory { get; private set; } = default!;

    private ICanReadState<TState> Reader { get; set; } = default!;

    internal void SetFactories(IGrainFactory grainFactory, IInternalQueryFactory queryFactory)
    {
        GrainFactory = grainFactory;
        QueryFactory = queryFactory;
    }

    void ISetGetState<TState>.SetGetState(ICanReadState<TState> reader)
    {
        Reader = reader;
    }
    void ISetAggregate.SetAggregate(GrainId aggregateId)
    {
        AggregateId = aggregateId;
    }
    internal override void SetGrainId(ComponentId addressable, GrainId grainId)
    {
        _address = addressable;
        GrainId = grainId;
    }


    public ValueTask<TState> GetState()
    {
        return Reader.GetState();
    }

}
