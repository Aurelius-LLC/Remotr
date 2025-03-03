namespace Remotr.Testing;

public class CqMockContainer : ICqMockContainer
{
    private Dictionary<Type, object> _types = new();

    public bool Get<TActual, TRequired>(out TRequired? implementation)
    {
        if (_types.ContainsKey(typeof(TActual)) && _types[typeof(TActual)] is TRequired required)
        {
            implementation = required;
            return true;
        }
        implementation = default;
        return false;
    }

    internal void AddOrUpdateTypes(Dictionary<Type, object> types)
    {
        _types = types;
    }
}

