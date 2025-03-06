using System.Text;

namespace Remotr.SourceGen.CqrsCollection.Generators;

/// <summary>
/// Generates code for command handlers.
/// </summary>
public class CommandHandlerGenerator
{
    /// <summary>
    /// Generates a stateless command handler with no input and no output.
    /// </summary>
    /// <param name="sb">The string builder to append to</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    public void GenerateNoInputNoOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType)
    {
        sb.AppendLine($"public class {className} : StatelessCommandHandler<{interfaceName}>");
        sb.AppendLine("{");
        sb.AppendLine("    public override async Task Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        await CommandFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Tell<{statefulHandlerName}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
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
    public void GenerateNoInputWithOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string outputType)
    {
        sb.AppendLine($"public class {className} : StatelessCommandHandler<{interfaceName}, {outputType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public override async Task<{outputType}> Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await CommandFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Tell<{statefulHandlerName}, {outputType}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
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
    public void GenerateWithInputAndOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string inputType,
        string outputType)
    {
        sb.AppendLine($"public class {className} : StatelessCommandHandler<{interfaceName}, {inputType}, {outputType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public override async Task<{outputType}> Execute({inputType} input)");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await CommandFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Tell<{statefulHandlerName}, {inputType}, {outputType}>(input)");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }
} 