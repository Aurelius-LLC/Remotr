namespace Remotr.SourceGen.HandlerAttributes.KeyStrategy;

/// <summary>
/// Key strategy that uses a fixed string value.
/// </summary>
public class FixedKeyStrategy : IHandlerKeyStrategy
{
    private readonly string _fixedKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedKeyStrategy"/> class.
    /// </summary>
    /// <param name="fixedKey">The fixed key to use.</param>
    public FixedKeyStrategy(string fixedKey)
    {
        _fixedKey = fixedKey;
    }

    /// <summary>
    /// Generates the key strategy code using a fixed string value.
    /// </summary>
    /// <returns>A string representing the fixed key value.</returns>
    public string GenerateKeyStrategy()
    {
        // Escape any quotes in the fixed key
        var escapedKey = _fixedKey.Replace("\"", "\\\"");
        return $"\"{escapedKey}\"";
    }
} 