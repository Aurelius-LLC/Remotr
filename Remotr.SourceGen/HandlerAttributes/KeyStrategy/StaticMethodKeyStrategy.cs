namespace Remotr.SourceGen.HandlerAttributes.KeyStrategy;

/// <summary>
/// Key strategy that calls a static method on the interface to generate the key.
/// </summary>
public class StaticMethodKeyStrategy : IHandlerKeyStrategy
{
    private readonly string _interfaceName;
    private readonly string _methodName;
    private readonly bool _hasInput;
    private readonly string _inputName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticMethodKeyStrategy"/> class.
    /// </summary>
    /// <param name="interfaceName">The interface name.</param>
    /// <param name="methodName">The static method name.</param>
    /// <param name="hasInput">Whether the method takes an input parameter.</param>
    /// <param name="inputName">The name of the input parameter, if needed.</param>
    public StaticMethodKeyStrategy(string interfaceName, string methodName, bool hasInput = false, string inputName = "")
    {
        _interfaceName = interfaceName;
        _methodName = methodName;
        _hasInput = hasInput;
        _inputName = inputName;
    }

    /// <summary>
    /// Generates the key strategy code by calling a static method on the interface.
    /// </summary>
    /// <returns>A string representing the static method call.</returns>
    public string GenerateKeyStrategy()
    {
        if (_hasInput)
        {
            return $"{_interfaceName}.{_methodName}({_inputName})";
        }
        else
        {
            return $"{_interfaceName}.{_methodName}()";
        }
    }
} 