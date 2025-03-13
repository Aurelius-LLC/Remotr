using System.Text;

namespace Remotr.StatefulExtensionGenerators;

/// <summary>
/// Component class that handles the generation of stateful handlers
/// </summary>
public class StatefulExtensionGeneratorComponent
{
    /// <summary>
    /// Configuration for generating handler extensions
    /// </summary>
    public class HandlerConfig
    {
        public string BaseBuilderType { get; set; }
        public string BuilderType { get; set; }
        public string HandlerType { get; set; }
        public string OtherHandlerType { get; set; }
        public bool IncludeOtherHandler { get; set; } = true;
    }

    /// <summary>
    /// Generates a stateful handler with no input and no output.
    /// </summary>
    public void GenerateNoInputNoOutput(
        StringBuilder sb,
        string className,
        string stateType,
        HandlerConfig config,
        string actionMethod)
    {
        var otherHandlerPart = config.IncludeOtherHandler ? $", BaseStateful{config.OtherHandlerType}Handler<{stateType}>" : "";
        
        sb.AppendLine($@"        public static {config.BaseBuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> {className}(this {config.BaseBuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> builder)
        {{
            return builder.{actionMethod}<{className}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static {config.BaseBuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> {className}<T>(this {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, T> builder)
        {{
            return builder.{actionMethod}<{className}>();
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
        HandlerConfig config,
        string actionMethod)
    {
        var otherHandlerPart = config.IncludeOtherHandler ? $", BaseStateful{config.OtherHandlerType}Handler<{stateType}>" : "";
        
        sb.AppendLine($@"        public static {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}(this {config.BaseBuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> builder)
        {{
            return builder.{actionMethod}<{className}, {outputType}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}<T>(this {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, T> builder)
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
        HandlerConfig config,
        string actionMethod,
        string thenActionMethod)
    {
        var otherHandlerPart = config.IncludeOtherHandler ? $", BaseStateful{config.OtherHandlerType}Handler<{stateType}>" : "";
        
        sb.AppendLine($@"        public static {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}(this {config.BaseBuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> builder, {inputType} input)
        {{
            return builder.{actionMethod}<{className}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}<T>(this {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, T> builder, {inputType} input)
        {{
            return builder.{actionMethod}<{className}, {inputType}, {outputType}>(input);
        }}");

        if (thenActionMethod != null)
        {
            sb.AppendLine();

            sb.AppendLine($@"        public static {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> Then{className}(this {config.BuilderType}<ITransactionChildGrain<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {inputType}> builder)
        {{
            return builder.{thenActionMethod}<{className}, {outputType}>();
        }}");
        }
    }
} 