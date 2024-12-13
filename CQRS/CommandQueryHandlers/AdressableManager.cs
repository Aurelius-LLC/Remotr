
using Orleans.Runtime;

namespace Remotr;

public abstract class AddressableManager
{
    public abstract IAddressable Address { get; }

    internal abstract void SetGrainId(IAddressable addressable, GrainId grainId);

    public Guid GetPrimaryKey()
    {
        return Address.GetPrimaryKey();
    }

    public Guid GetPrimaryKey(out string keyExt)
    {
        return Address.GetPrimaryKey(out keyExt);
    }

    public long GetPrimaryKeyLong()
    {
        return Address.GetPrimaryKeyLong();
    }

    public long GetPrimaryKeyLong(out string keyExt)
    {
        return Address.GetPrimaryKeyLong(out keyExt);
    }

    public string GetPrimaryKeyString()
    {
        var stringKey = Address.GetPrimaryKeyString();
        var isGuid = Guid.TryParse(stringKey, out var guidKey);
        return isGuid ? guidKey.ToString() : stringKey;
    }
}
