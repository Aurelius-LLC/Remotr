using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Remotr.SourceGen;

/// <summary>
/// Generator for StatefulCommandHandler types that generates appropriate extension methods.
/// </summary>
public class StatefulCommandHandlerGenerator : BaseHandlerGenerator
{
    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatefulCommandHandler";

    /// <inheritdoc/>
    public override void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        if (!ValidateTypeArgumentCount(typeArguments, 1, 3))
            return;

        var t1 = typeArguments[0].ToString();

        switch (typeArguments.Count)
        {
            case 1:
                GenerateNoInputNoOutput(sb, className, t1);
                break;
            case 2:
                GenerateNoInputWithOutput(sb, className, t1, typeArguments[1].ToString());
                break;
            case 3:
                GenerateWithInputAndOutput(sb, className, t1, typeArguments[1].ToString(), typeArguments[2].ToString());
                break;
        }
    }

    private void GenerateNoInputNoOutput(StringBuilder sb, string className, string t1)
    {
        sb.AppendLine($@"        public static IGrainCommandBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>> builder)
        {{
            return builder.Tell<{className}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, T> builder)
        {{
            return builder.Tell<{className}>();
        }}");
    }

    private void GenerateNoInputWithOutput(StringBuilder sb, string className, string t1, string t2)
    {
        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>> builder)
        {{
            return builder.Tell<{className}, {t2}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, T> builder)
        {{
            return builder.Tell<{className}, {t2}>();
        }}");
    }

    private void GenerateWithInputAndOutput(StringBuilder sb, string className, string t1, string t2, string t3)
    {
        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>> builder, {t2} input)
        {{
            return builder.Tell<{className}, {t2}, {t3}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, T> builder, {t2} input)
        {{
            return builder.Tell<{className}, {t2}, {t3}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> Then{className}(this IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> builder)
        {{
            return builder.ThenTell<{className}, {t3}>();
        }}");
    }
} 