using Remotr.SourceGen.UseHandlerAttributes.KeyStrategy;
using System.Text;

namespace Remotr.SourceGen.UseHandlerAttributes.HandlerGenerators;

/// <summary>
/// Interface for stateless handler generators.
/// </summary>
public interface IStatelessHandlerGenerator
{
    /// <summary>
    /// Generates a stateless handler with no input and no output.
    /// </summary>
    /// <param name="sourceBuilder">The source builder</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="keyStrategy">The key generation strategy</param>
    void GenerateNoInputNoOutput(
        StringBuilder sourceBuilder, 
        string interfaceName, 
        string className, 
        string statefulHandlerName, 
        string stateType,
        IHandlerKeyStrategy keyStrategy);

    /// <summary>
    /// Generates a stateless handler with no input and with output.
    /// </summary>
    /// <param name="sourceBuilder">The source builder</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="outputType">The output type</param>
    /// <param name="keyStrategy">The key generation strategy</param>
    void GenerateNoInputWithOutput(
        StringBuilder sourceBuilder, 
        string interfaceName, 
        string className, 
        string statefulHandlerName, 
        string stateType, 
        string outputType,
        IHandlerKeyStrategy keyStrategy);

    /// <summary>
    /// Generates a stateless handler with input and output.
    /// </summary>
    /// <param name="sourceBuilder">The source builder</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="inputType">The input type</param>
    /// <param name="outputType">The output type</param>
    /// <param name="keyStrategy">The key generation strategy</param>
    void GenerateWithInputAndOutput(
        StringBuilder sourceBuilder, 
        string interfaceName, 
        string className, 
        string statefulHandlerName, 
        string stateType, 
        string inputType, 
        string outputType,
        IHandlerKeyStrategy keyStrategy);
} 