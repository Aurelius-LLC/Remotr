using Orleans.Runtime;

namespace Remotr;

public abstract class AddressableChild
{
    public abstract ComponentId Address { get; }

    internal abstract void SetGrainId(ComponentId addressable, GrainId grainId);

    public string EntityKey => Address.ItemId;
}
