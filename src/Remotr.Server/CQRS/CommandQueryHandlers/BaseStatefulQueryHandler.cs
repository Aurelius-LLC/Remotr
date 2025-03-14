using Orleans.Runtime;

namespace Remotr;

public abstract class BaseStatefulQueryHandler<TState> : AddressableChild, ISetGetState<TState>, ISetManagerGrain, IDiscoverableCq
    where TState : new()
{
    public GrainId ManagerGrainId { get; private set; } = default!;
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
    void ISetManagerGrain.SetManagerGrain(GrainId managerGrainId)
    {
        ManagerGrainId = managerGrainId;
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
