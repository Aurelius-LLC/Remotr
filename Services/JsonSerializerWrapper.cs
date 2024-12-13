using System.Text.Json;

namespace Remotr;

public sealed class JsonSerializerWrapper : IJsonSerializerWrapper
{
    private readonly JsonSerializerOptions _options;

    public JsonSerializerWrapper(JsonSerializerOptions options)
    {
        _options = options;
    }

    public string Serialize<TValue>(TValue value, JsonSerializerOptions? options = null)
    {
        if (options != null)
        {
            return JsonSerializer.Serialize(value, options);
        }
        else
        {
            return JsonSerializer.Serialize(value, _options);
        }
    }
}
