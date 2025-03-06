using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Source generator for Remotr that generates extension methods for various handler types.
/// This generator processes classes marked with the RemotrGen attribute and generates
/// appropriate extension methods based on the handler type.
/// </summary>
[Generator]
public class RemotrGenerator : IIncrementalGenerator
{
    private readonly IReadOnlyList<IExtensionGenerator> _handlerGenerators;

    public RemotrGenerator()
    {
        _handlerGenerators = new List<IExtensionGenerator>
        {
            new StatelessCommandHandlerGenerator(),
            new StatelessQueryHandlerGenerator(),
            new StatefulCommandHandlerGenerator(),
            new StatefulQueryHandlerGenerator()
        };
    }

    /// <summary>
    /// Initializes the generator and registers the necessary syntax providers and outputs.
    /// </summary>
    /// <param name="context">The generator initialization context</param>
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
            (spc, source) => Execute(source, spc));
    }

    /// <summary>
    /// Determines if a syntax node should be considered for code generation.
    /// </summary>
    /// <param name="node">The syntax node to check</param>
    /// <returns>True if the node is a class declaration with attributes</returns>
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is ClassDeclarationSyntax { AttributeLists: { Count: > 0 } };

    /// <summary>
    /// Gets the semantic model target for code generation.
    /// </summary>
    /// <param name="context">The generator syntax context</param>
    /// <returns>The class declaration if it has the RemotrGen attribute, null otherwise</returns>
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

    /// <summary>
    /// Executes the code generation for a class declaration.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to process</param>
    /// <param name="context">The source production context</param>
    private void Execute(ClassDeclarationSyntax classDeclaration, SourceProductionContext context)
    {
        var className = classDeclaration.Identifier.Text;
        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
        
        if (baseType is not GenericNameSyntax genericBase)
        {
            return;
        }

        var baseTypeName = genericBase.Identifier.Text;
        var typeArguments = genericBase.TypeArgumentList.Arguments;

        // Get the namespace from the source file
        var namespaceDecl = classDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        var namespaceName = namespaceDecl?.Name.ToString()!;

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

        var handlerGenerator = _handlerGenerators.FirstOrDefault(g => g.CanHandle(baseTypeName));
        handlerGenerator?.GenerateExtensions(sourceBuilder, className, typeArguments);

        sourceBuilder.AppendLine("}");

        context.AddSource($"{extensionClassName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }
} 