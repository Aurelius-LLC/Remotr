namespace Remotr.SourceGen.HandlerAttributes.KeyStrategy;

/// <summary>
/// Interface for handler key generation strategies.
/// </summary>
public interface IHandlerKeyStrategy
{
    /// <summary>
    /// Generates the key strategy code that will be used in the handler.
    /// </summary>
    /// <returns>A string representing the code to generate the key.</returns>
    string GenerateKeyStrategy();
} 