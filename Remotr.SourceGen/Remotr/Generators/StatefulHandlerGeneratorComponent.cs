using System.Text;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Component class that handles the generation of stateful handlers
/// </summary>
public class StatefulHandlerGeneratorComponent
{
    /// <summary>
    /// Generates a stateful handler with no input and no output.
    /// </summary>
    public void GenerateNoInputNoOutput(
        StringBuilder sb,
        string className,
        string stateType,
        string handlerType)
    {
        sb.AppendLine($@"        public static IGrainCommandBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>> builder)
        {{
            return builder.Tell<{className}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, T> builder)
        {{
            return builder.Tell<{className}>();
        }}");
    }

    /// <summary>
    /// Generates a stateful handler with no input but with output.
    /// </summary>
    public void GenerateNoInputWithOutput(
        StringBuilder sb,
        string className,
        string stateType,
        string outputType,
        string handlerType,
        string actionMethod)
    {
        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>> builder)
        {{
            return builder.{actionMethod}<{className}, {outputType}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, T> builder)
        {{
            return builder.{actionMethod}<{className}, {outputType}>();
        }}");
    }

    /// <summary>
    /// Generates a stateful handler with input and output.
    /// </summary>
    public void GenerateWithInputAndOutput(
        StringBuilder sb,
        string className,
        string stateType,
        string inputType,
        string outputType,
        string handlerType,
        string actionMethod,
        string thenActionMethod)
    {
        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>> builder, {inputType} input)
        {{
            return builder.{actionMethod}<{className}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, T> builder, {inputType} input)
        {{
            return builder.{actionMethod}<{className}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> Then{className}(this IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStateful{handlerType}Handler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {inputType}> builder)
        {{
            return builder.{thenActionMethod}<{className}, {outputType}>();
        }}");
    }
} 