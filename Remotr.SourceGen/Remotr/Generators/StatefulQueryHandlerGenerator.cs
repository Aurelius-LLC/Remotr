using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Generator for StatefulQueryHandler types that generates appropriate extension methods.
/// </summary>
public class StatefulQueryHandlerGenerator : BaseExtensionGenerator, IStatefulHandlerGenerator
{
    private readonly StatefulHandlerGeneratorComponent _component;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulQueryHandlerGenerator"/> class.
    /// </summary>
    public StatefulQueryHandlerGenerator()
    {
        _component = new StatefulHandlerGeneratorComponent();
    }

    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatefulQueryHandler";

    /// <inheritdoc/>
    public override void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        if (!ValidateTypeArgumentCount(typeArguments, 2, 3))
            return;

        var t1 = typeArguments[0].ToString();

        switch (typeArguments.Count)
        {
            case 2:
                GenerateNoInput(sb, className, t1, typeArguments[1].ToString());
                break;
            case 3:
                GenerateWithInput(sb, className, t1, typeArguments[1].ToString(), typeArguments[2].ToString());
                break;
        }
    }

    /// <inheritdoc/>
    public void GenerateNoInputNoOutput(StringBuilder sb, string className, string stateType)
    {
        throw new NotImplementedException("Query handlers must have at least an output.");
    }

    /// <inheritdoc/>
    public void GenerateNoInputWithOutput(StringBuilder sb, string className, string stateType, string outputType)
    {
        GenerateNoInput(sb, className, stateType, outputType);
    }

    /// <inheritdoc/>
    public void GenerateWithInputAndOutput(StringBuilder sb, string className, string stateType, string inputType, string outputType)
    {
        GenerateWithInput(sb, className, stateType, inputType, outputType);
    }

    private void GenerateNoInput(StringBuilder sb, string className, string stateType, string outputType)
    {
        sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}(this IGrainQueryBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>> builder)
        {{
            return builder.Ask<{className}, {outputType}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>> builder)
        {{
            return builder.Ask<{className}, {outputType}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}<T>(this IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, T> builder)
        {{
            return builder.Ask<{className}, {outputType}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, T> builder)
        {{
            return builder.Ask<{className}, {outputType}>();
        }}");
    }

    private void GenerateWithInput(StringBuilder sb, string className, string stateType, string inputType, string outputType)
    {
        sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}(this IGrainQueryBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>> builder, {inputType} input)
        {{
            return builder.Ask<{className}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>> builder, {inputType} input)
        {{
            return builder.Ask<{className}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}<T>(this IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, T> builder, {inputType} input)
        {{
            return builder.Ask<{className}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, T> builder, {inputType} input)
        {{
            return builder.Ask<{className}, {inputType}, {outputType}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> Then{className}(this IGrainQueryBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {inputType}> builder)
        {{
            return builder.ThenAsk<{className}, {outputType}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {outputType}> Then{className}(this IGrainCommandBuilder<ITransactionChildGrain<{stateType}>, BaseStatefulCommandHandler<{stateType}>, BaseStatefulQueryHandler<{stateType}>, {inputType}> builder)
        {{
            return builder.ThenAsk<{className}, {outputType}>();
        }}");
    }
} 