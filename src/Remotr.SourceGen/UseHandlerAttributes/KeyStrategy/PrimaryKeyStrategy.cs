namespace Remotr.SourceGen.UseHandlerAttributes.KeyStrategy;

/// <summary>
/// Key strategy that uses the default GetPrimaryKeyString() method.
/// </summary>
public class PrimaryKeyStrategy : IHandlerKeyStrategy
{
    /// <summary>
    /// Generates the key strategy code using GetPrimaryKeyString().
    /// </summary>
    /// <returns>A string representing the GetPrimaryKeyString() method call.</returns>
    public string GenerateKeyStrategy()
    {
        return "GetPrimaryKeyString()";
    }
} 