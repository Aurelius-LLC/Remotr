namespace Remotr;

public interface IAddressableFacade
{
    Guid GetPrimaryKey();
    Guid GetPrimaryKey(out string keyExt);
    string GetPrimaryKeyString();
    long GetPrimaryKeyLong();
    long GetPrimaryKeyLong(out string keyExt);
}
