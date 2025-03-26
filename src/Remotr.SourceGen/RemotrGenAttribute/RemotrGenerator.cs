using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotr.SourceGen.UseShortcutsAttribute.Utils;

namespace Remotr.SourceGen.UseShortcutsAttribute;

/// <summary>
/// Source generator for Remotr that generates extension methods for various handler types.
/// This generator processes classes marked with the UseShortcuts attribute and generates
/// appropriate extension methods based on the handler type.
/// </summary>
[Generator]
public class UseShortcutserator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the generator and registers the necessary syntax providers and outputs.
    /// </summary>
    /// <param name="context">The generator initialization context</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        var attributeGenerator = new RemotrAttributeGenerator();
        attributeGenerator.RegisterAttributeSource(context);

        // Get all class declarations with the UseShortcuts attribute
        IncrementalValuesProvider<(ClassDeclarationSyntax ClassDeclaration, bool IsValid)> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => SyntaxTargetIdentifier.IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => SemanticTargetIdentifier.GetSemanticTargetForGeneration(ctx))
            .Where(static m => m.ClassDeclaration is not null)!;

        // Register the source output
        context.RegisterSourceOutput(classDeclarations,
            (spc, source) => new RemotrExecutor().Execute(source.ClassDeclaration, source.IsValid, spc));
    }
} 