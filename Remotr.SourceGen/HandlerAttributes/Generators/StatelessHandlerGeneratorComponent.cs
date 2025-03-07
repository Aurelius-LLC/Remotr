using System.Text;

namespace Remotr.SourceGen.HandlerAttributes.Generators
{
    /// <summary>
    /// Component class that handles the generation of stateless handlers
    /// </summary>
    public class StatelessHandlerGeneratorComponent
    {
        /// <summary>
        /// Generates a stateless handler with no input but with output.
        /// </summary>
        public void GenerateNoInputWithOutput(
            StringBuilder sb,
            string interfaceName,
            string className,
            string statefulHandlerName,
            string stateType,
            string outputType,
            string handlerType,
            string factoryType,
            string actionMethod)
        {
            sb.AppendLine($"public class {className} : Stateless{handlerType}Handler<{interfaceName}, {outputType}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public override async Task<{outputType}> Execute()");
            sb.AppendLine("    {");
            sb.AppendLine($"        return await {factoryType}Factory.GetChild<{stateType}>()");
            sb.AppendLine($"            .{actionMethod}<{statefulHandlerName}, {outputType}>()");
            sb.AppendLine("            .Run(GetPrimaryKeyString());");
            sb.AppendLine("    }");
            sb.AppendLine("}");
        }

        /// <summary>
        /// Generates a stateless handler with input and output.
        /// </summary>
        public void GenerateWithInputAndOutput(
            StringBuilder sb,
            string interfaceName,
            string className,
            string statefulHandlerName,
            string stateType,
            string inputType,
            string outputType,
            string handlerType,
            string factoryType,
            string actionMethod)
        {
            sb.AppendLine($"public class {className} : Stateless{handlerType}Handler<{interfaceName}, {inputType}, {outputType}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public override async Task<{outputType}> Execute({inputType} input)");
            sb.AppendLine("    {");
            sb.AppendLine($"        return await {factoryType}Factory.GetChild<{stateType}>()");
            sb.AppendLine($"            .{actionMethod}<{statefulHandlerName}, {inputType}, {outputType}>(input)");
            sb.AppendLine("            .Run(GetPrimaryKeyString());");
            sb.AppendLine("    }");
            sb.AppendLine("}");
        }
    }
} 