
using Orleans.Runtime;

namespace Remotr;

public abstract class AddressableManager
{
    public abstract IAddressable Address { get; }

    internal abstract void SetGrainId(IAddressable addressable, GrainId grainId);

    public Guid GetRootKey()
    {
        return Address.GetPrimaryKey();
    }

    public Guid GetRootKey(out string keyExt)
    {
        return Address.GetPrimaryKey(out keyExt);
    }

    public long GetRootKeyLong()
    {
        return Address.GetPrimaryKeyLong();
    }

    public long GetRootKeyLong(out string keyExt)
    {
        return Address.GetPrimaryKeyLong(out keyExt);
    }

    public string GetRootKeyString()
    {
        var stringKey = Address.GetPrimaryKeyString();
        var isGuid = Guid.TryParse(stringKey, out var guidKey);
        return isGuid ? guidKey.ToString() : stringKey;
    }
}
