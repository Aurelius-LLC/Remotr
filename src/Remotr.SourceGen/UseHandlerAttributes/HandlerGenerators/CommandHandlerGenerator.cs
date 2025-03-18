using Remotr.SourceGen.UseHandlerAttributes.KeyStrategy;
using System.Text;

namespace Remotr.SourceGen.UseHandlerAttributes.HandlerGenerators;

/// <summary>
/// Generator for command handlers
/// </summary>
public class CommandHandlerGenerator : IStatelessHandlerGenerator
{
    private readonly StatelessHandlerGeneratorComponent _component;

    public CommandHandlerGenerator()
    {
        _component = new StatelessHandlerGeneratorComponent();
    }

    /// <summary>
    /// Generates a stateless command handler with no input and no output.
    /// </summary>
    /// <param name="sb">The string builder to append to</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="keyStrategy">The key generation strategy</param>
    public void GenerateNoInputNoOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string genericTypeArgsString,
        IHandlerKeyStrategy keyStrategy)
    {
        sb.AppendLine($"public class {className} : RootCommandHandler<{interfaceName}>");
        sb.AppendLine("{");
        sb.AppendLine("    public override async Task Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        await CommandFactory.GetEntity<{stateType}>()");
        sb.AppendLine($"            .Tell<{statefulHandlerName}{genericTypeArgsString}>()");
        sb.AppendLine($"            .Run({keyStrategy.GenerateKeyStrategy()});");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }

    /// <summary>
    /// Generates a stateless command handler with no input but with output.
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
        string genericTypeArgsString,
        IHandlerKeyStrategy keyStrategy)
    {
        _component.GenerateNoInputWithOutput(
            sb,
            interfaceName,
            className,
            statefulHandlerName,
            stateType,
            outputType,
            "Command",
            "Command",
            "Tell",
            genericTypeArgsString,
            keyStrategy);
    }

    /// <summary>
    /// Generates a stateless command handler with input and output.
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
        string genericTypeArgsString,
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
            "Command",
            "Command",
            "Tell",
            genericTypeArgsString,
            keyStrategy);
    }
} 