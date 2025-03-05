using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using System.Linq;

namespace Remotr.SourceGen;

[Generator]
public class RemotrGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "RemotrGenAttribute.g.cs",
            SourceText.From(@"
using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RemotrGenAttribute : Attribute
    {
        public RemotrGenAttribute() { }
    }
}", Encoding.UTF8)));

        // Get all class declarations with the RemotrGen attribute
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        // Register the source output
        context.RegisterSourceOutput(classDeclarations,
            static (spc, source) => Execute(source, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is ClassDeclarationSyntax { AttributeLists: { Count: > 0 } };

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Remotr.RemotrGenAttribute")
                {
                    return classDeclaration;
                }
            }
        }

        return null;
    }

    private static void Execute(ClassDeclarationSyntax classDeclaration, SourceProductionContext context)
    {
        var className = classDeclaration.Identifier.Text;
        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
        
        if (baseType is not GenericNameSyntax genericBase)
        {
            return;
        }

        var baseTypeName = genericBase.Identifier.Text;
        var typeArguments = genericBase.TypeArgumentList.Arguments;
        var typeArgumentCount = typeArguments.Count;

        if (typeArgumentCount < 1 || typeArgumentCount > 3)
        {
            return;
        }

        if ((baseTypeName == "StatelessQueryHandler" || baseTypeName == "StatefulQueryHandler") && typeArgumentCount < 2)
        {
            return;
        }

        // Get the namespace from the source file
        var namespaceDecl = classDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        var namespaceName = namespaceDecl?.Name.ToString() ?? "Remotr.Example.Calculator";

        var sourceBuilder = new StringBuilder();
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using System.Threading.Tasks;");
        sourceBuilder.AppendLine("using Remotr;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine($"namespace {namespaceName};");
        sourceBuilder.AppendLine();

        var t1 = typeArguments[0].ToString();
        var extensionClassName = $"{t1}{className}";

        sourceBuilder.AppendLine($"public static class {extensionClassName}");
        sourceBuilder.AppendLine("{");

        switch (baseTypeName)
        {
            case "StatelessCommandHandler":
                GenerateStatelessCommandHandlerExtensions(sourceBuilder, className, typeArguments);
                break;
            case "StatelessQueryHandler":
                GenerateStatelessQueryHandlerExtensions(sourceBuilder, className, typeArguments);
                break;
            case "StatefulCommandHandler":
                GenerateStatefulCommandHandlerExtensions(sourceBuilder, className, typeArguments);
                break;
            case "StatefulQueryHandler":
                GenerateStatefulQueryHandlerExtensions(sourceBuilder, className, typeArguments);
                break;
        }

        sourceBuilder.AppendLine("}");

        context.AddSource($"{extensionClassName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    private static void GenerateStatelessCommandHandlerExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        var t1 = typeArguments[0].ToString();
        
        if (typeArguments.Count == 1)
        {
            sb.AppendLine($@"        public static IGrainCommandBaseBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>> {className}(this IGrainCommandBaseBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>> builder)
        {{
            return builder.Tell<{className}>();
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBaseBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>> {className}<T>(this IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, T> builder)
        {{
            return builder.Tell<{className}>();
        }}");
        }
        else if (typeArguments.Count == 2)
        {
            var t2 = typeArguments[1].ToString();

            sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t2}> {className}(this IGrainCommandBaseBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>> builder)
        {{
            return builder.Tell<{className}, {t2}>();
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t2}> {className}<T>(this IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, T> builder)
        {{
            return builder.Tell<{className}, {t2}>();
        }}");
        }
        else if (typeArguments.Count == 3)
        {
            var t2 = typeArguments[1].ToString();
            var t3 = typeArguments[2].ToString();

            sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t3}> {className}(this IGrainCommandBaseBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>> builder, {t2} input)
        {{
            return builder.Tell<{className}, {t2}, {t3}>(input);
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t3}> {className}<T>(this IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, T> builder, {t2} input)
        {{
            return builder.Tell<{className}, {t2}, {t3}>(input);
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t3}> Then{className}(this IGrainCommandBuilder<{t1}, BaseStatelessCommandHandler<{t1}>, BaseStatelessQueryHandler<{t1}>, {t2}> builder)
        {{
            return builder.ThenTell<{className}, {t3}>();
        }}");
        }
    }

    private static void GenerateStatelessQueryHandlerExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        var t1 = typeArguments[0].ToString();
        
        if (typeArguments.Count == 2)
        {
            var t2 = typeArguments[1].ToString();

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
        else if (typeArguments.Count == 3)
        {
            var t2 = typeArguments[1].ToString();
            var t3 = typeArguments[2].ToString();

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

    private static void GenerateStatefulCommandHandlerExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        var t1 = typeArguments[0].ToString();
        
        if (typeArguments.Count == 1)
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
        else if (typeArguments.Count == 2)
        {
            var t2 = typeArguments[1].ToString();

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
        else if (typeArguments.Count == 3)
        {
            var t2 = typeArguments[1].ToString();
            var t3 = typeArguments[2].ToString();

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

    private static void GenerateStatefulQueryHandlerExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        var t1 = typeArguments[0].ToString();
        
        if (typeArguments.Count == 2)
        {
            var t2 = typeArguments[1].ToString();

            sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> {className}(this IGrainQueryBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> {className}<T>(this IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, T> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, T> builder)
        {{
            return builder.Ask<{className}, {t2}>();
        }}");
        }
        else if (typeArguments.Count == 3)
        {
            var t2 = typeArguments[1].ToString();
            var t3 = typeArguments[2].ToString();

            sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> {className}(this IGrainQueryBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> {className}(this IGrainCommandBaseBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> {className}<T>(this IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, T> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> {className}<T>(this IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, T> builder, {t2} input)
        {{
            return builder.Ask<{className}, {t2}, {t3}>(input);
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> Then{className}(this IGrainQueryBuilder<ITransactionChildGrain<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> builder)
        {{
            return builder.ThenAsk<{className}, {t3}>();
        }}");

            sb.AppendLine();

            sb.AppendLine($@"        public static IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t3}> Then{className}(this IGrainCommandBuilder<ITransactionChildGrain<{t1}>, BaseStatefulCommandHandler<{t1}>, BaseStatefulQueryHandler<{t1}>, {t2}> builder)
        {{
            return builder.ThenAsk<{className}, {t3}>();
        }}");
        }
    }
} 