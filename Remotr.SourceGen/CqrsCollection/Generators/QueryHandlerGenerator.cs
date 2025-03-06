using System.Text;

namespace Remotr.SourceGen.CqrsCollection.Generators;

/// <summary>
/// Generates code for query handlers.
/// </summary>
public class QueryHandlerGenerator
{
    /// <summary>
    /// Generates a stateless query handler with no output.
    /// </summary>
    /// <param name="sb">The string builder to append to</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    public void GenerateNoOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType)
    {
        sb.AppendLine($"public class {className} : StatelessQueryHandler<{interfaceName}>");
        sb.AppendLine("{");
        sb.AppendLine("    public override async Task Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        await QueryFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Ask<{statefulHandlerName}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }

    /// <summary>
    /// Generates a stateless query handler with output.
    /// </summary>
    /// <param name="sb">The string builder to append to</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="className">The class name</param>
    /// <param name="statefulHandlerName">The stateful handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="outputType">The output type</param>
    public void GenerateWithOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string outputType)
    {
        sb.AppendLine($"public class {className} : StatelessQueryHandler<{interfaceName}, {outputType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public override async Task<{outputType}> Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await QueryFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Ask<{statefulHandlerName}, {outputType}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
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
    public void GenerateWithInputAndOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string inputType,
        string outputType)
    {
        sb.AppendLine($"public class {className} : StatelessQueryHandler<{interfaceName}, {inputType}, {outputType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public override async Task<{outputType}> Execute({inputType} input)");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await QueryFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Ask<{statefulHandlerName}, {inputType}, {outputType}>(input)");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }
} 