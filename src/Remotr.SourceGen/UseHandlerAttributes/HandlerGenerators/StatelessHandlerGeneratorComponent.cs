using Remotr.SourceGen.UseHandlerAttributes.KeyStrategy;
using System.Text;

namespace Remotr.SourceGen.UseHandlerAttributes.HandlerGenerators
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
            string actionMethod,
            string genericTypeArgsString,
            IHandlerKeyStrategy keyStrategy)
        {
            sb.AppendLine($"public class {className} : Root{handlerType}Handler<{interfaceName}, {outputType}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public override async Task<{outputType}> Execute()");
            sb.AppendLine("    {");
            sb.AppendLine($"        return await {factoryType}Factory.GetEntity<{stateType}>()");
            sb.AppendLine($"            .{actionMethod}<{statefulHandlerName}{genericTypeArgsString}, {outputType}>()");
            sb.AppendLine($"            .Run({keyStrategy.GenerateKeyStrategy()});");
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
            string actionMethod,
            string genericTypeArgsString,
            IHandlerKeyStrategy keyStrategy)
        {
            sb.AppendLine($"public class {className} : Root{handlerType}Handler<{interfaceName}, {inputType}, {outputType}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public override async Task<{outputType}> Execute({inputType} input)");
            sb.AppendLine("    {");
            sb.AppendLine($"        return await {factoryType}Factory.GetEntity<{stateType}>()");
            sb.AppendLine($"            .{actionMethod}<{statefulHandlerName}{genericTypeArgsString}, {inputType}, {outputType}>(input)");
            sb.AppendLine($"            .Run({keyStrategy.GenerateKeyStrategy()});");
            sb.AppendLine("    }");
            sb.AppendLine("}");
        }
    }
} 