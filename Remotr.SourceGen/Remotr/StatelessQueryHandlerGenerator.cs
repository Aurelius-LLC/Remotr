using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Generator for StatelessQueryHandler types that generates appropriate extension methods.
/// </summary>
public class StatelessQueryHandlerGenerator : BaseHandlerGenerator
{
    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatelessQueryHandler";

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

    private void GenerateNoInput(StringBuilder sb, string className, string t1, string t2)
    {
        sb.AppendLine($@"        public static IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, {t2}> {className}(this IGrainQueryBaseBuilder<{t1}, BaseStatelessQueryHandler<{t1}>> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t2}> {className}(this IGrainCommandBaseBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, {t2}> {className}<T>(this IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, T> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t2}> {className}<T>(this IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, T> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");
    }

    private void GenerateWithInput(StringBuilder sb, string className, string t1, string t2, string t3)
    {
        sb.AppendLine($@"        public static IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, {t3}> {className}(this IGrainQueryBaseBuilder<{t1}, BaseStatelessQueryHandler<{t1}>> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, {t3}> {className}<T>(this IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, T> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t3}> {className}(this IGrainCommandBaseBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t3}> {className}<T>(this IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, T> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, {t3}> Then{className}(this IGrainQueryBuilder<{t1}, BaseStatelessQueryHandler<{t1}>, {t2}> builder)
        {{
            return builder.ThenAsk<{className}, {t3}>();
        }}");

        sb.AppendLine();

        sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t3}> Then{className}(this IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t2}> builder)
        {{
            return builder.ThenAsk<{className}, {t3}>();
        }}");
    }
} 