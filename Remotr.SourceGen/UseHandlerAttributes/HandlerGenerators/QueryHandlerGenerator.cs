using Remotr.SourceGen.UseHandlerAttributes.KeyStrategy;
using System.Text;

namespace Remotr.SourceGen.UseHandlerAttributes.HandlerGenerators;

/// <summary>
/// Generator for query handlers
/// </summary>
public class QueryHandlerGenerator : IStatelessHandlerGenerator
{
    private readonly StatelessHandlerGeneratorComponent _component;

    public QueryHandlerGenerator()
    {
        _component = new StatelessHandlerGeneratorComponent();
    }

    /// <summary>
    /// This shouldn't be called as query handlers can't have no input and no output.
    /// </summary>
    public void GenerateNoInputNoOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName, 
        string stateType, 
        IHandlerKeyStrategy keyStrategy)
    {
        throw new NotImplementedException("Query handlers must have at least an output.");
    }

    /// <summary>
    /// Generates a stateless query handler with no input but with output.
    /// </summary>
    /// <param name="sb">The string builder to append to</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="outputType">The output type</param>
    /// <param name="keyStrategy">The key generation strategy</param>
    public void GenerateNoInputWithOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string outputType,
        IHandlerKeyStrategy keyStrategy)
    {
        _component.GenerateNoInputWithOutput(
            sb,
            interfaceName,
            className,
            statefulHandlerName,
            stateType,
            outputType,
            "Query",
            "Query",
            "Ask",
            keyStrategy);
    }

    /// <summary>
    /// Generates a stateless query handler with input and output.
    /// </summary>
    /// <param name="sb">The string builder to append to</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="inputType">The input type</param>
    /// <param name="outputType">The output type</param>
    /// <param name="keyStrategy">The key generation strategy</param>
    public void GenerateWithInputAndOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string inputType,
        string outputType,
        IHandlerKeyStrategy keyStrategy)
    {
        _component.GenerateWithInputAndOutput(
            sb,
            interfaceName,
            className,
            statefulHandlerName,
            stateType,
            inputType,
            outputType,
            "Query",
            "Query",
            "Ask",
            keyStrategy);
    }
} 