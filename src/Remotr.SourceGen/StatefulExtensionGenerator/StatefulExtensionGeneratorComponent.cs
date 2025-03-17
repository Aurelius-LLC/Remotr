using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.StatefulExtensionGenerators;

/// <summary>
/// Component class that handles the generation of stateful handlers
/// </summary>
public class StatefulExtensionGeneratorComponent
{

    /// <summary>
    /// Generates a stateful handler with no input and no output.
    /// </summary>
    public void GenerateNoInputNoOutput(
        StringBuilder sb,
        string className,
        string stateType,
        ExtensionConfig config,
        string actionMethod,
        string implementationGenericTypes,
        string implementationGenericTypesWithOther,
        string genericConstraints,
        string otherGenericType)
    {
        var otherHandlerPart = config.IncludeOtherHandler ? $", BaseStateful{config.OtherHandlerType}Handler<{stateType}>" : "";
        
        sb.AppendLine($@"        public static {config.BaseBuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> {className}{implementationGenericTypes}(this {config.BaseBuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> builder){genericConstraints}
        {{
            return builder.{actionMethod}<{className}{implementationGenericTypes}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static {config.BaseBuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> {className}{implementationGenericTypesWithOther}(this {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {otherGenericType}> builder){genericConstraints}
        {{
            return builder.{actionMethod}<{className}{implementationGenericTypes}>();
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
        ExtensionConfig config,
        string actionMethod,
        string implementationGenericTypes,
        string implementationGenericTypesWithOther,
        string genericConstraints,
        string otherGenericType)
    {
        var otherHandlerPart = config.IncludeOtherHandler ? $", BaseStateful{config.OtherHandlerType}Handler<{stateType}>" : "";
        
        sb.AppendLine($@"        public static {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}{implementationGenericTypes}(this {config.BaseBuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> builder){genericConstraints}
        {{
            return builder.{actionMethod}<{className}{implementationGenericTypes}, {outputType}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}{implementationGenericTypesWithOther}(this {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {otherGenericType}> builder){genericConstraints}
        {{
            return builder.{actionMethod}<{className}{implementationGenericTypes}, {outputType}>();
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
        ExtensionConfig config,
        string actionMethod,
        string thenActionMethod,
        string implementationGenericTypes,
        string implementationGenericTypesWithOther,
        string genericConstraints,
        string otherGenericType)
    {
        var otherHandlerPart = config.IncludeOtherHandler ? $", BaseStateful{config.OtherHandlerType}Handler<{stateType}>" : "";
        
        sb.AppendLine($@"        public static {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}{implementationGenericTypes}(this {config.BaseBuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}> builder, {inputType} input){genericConstraints}
        {{
            return builder.{actionMethod}<{className}{implementationGenericTypes}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> {className}{implementationGenericTypesWithOther}(this {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {otherGenericType}> builder, {inputType} input){genericConstraints}
        {{
            return builder.{actionMethod}<{className}{implementationGenericTypes}, {inputType}, {outputType}>(input);
        }}");

        if (thenActionMethod != null)
        {
            sb.AppendLine();

            sb.AppendLine($@"        public static {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {outputType}> Then{className}{implementationGenericTypes}(this {config.BuilderType}<IAggregateEntity<{stateType}>, BaseStateful{config.HandlerType}Handler<{stateType}>{otherHandlerPart}, {inputType}> builder){genericConstraints}
        {{
            return builder.{thenActionMethod}<{className}{implementationGenericTypes}, {outputType}>();
        }}");
        }
    }
} 