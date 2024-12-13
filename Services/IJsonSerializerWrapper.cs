using System.Text.Json;

namespace Remotr;

public interface IJsonSerializerWrapper
{
    public string Serialize<TValue>(TValue value, JsonSerializerOptions? options = null);
}
