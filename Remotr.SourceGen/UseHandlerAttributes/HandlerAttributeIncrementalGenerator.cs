using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotr.SourceGen.UseHandlerAttributes.Utils;
using Remotr.SourceGen.Shared;

namespace Remotr.SourceGen.UseHandlerAttributes;

[Generator]
public class HandlerAttributeIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        var attributeGenerator = new AttributeGenerator();
        attributeGenerator.RegisterAttributeSource(context);

        // Get all interface declarations with the UseCommand or UseQuery attributes
        IncrementalValueProvider<Compilation> compilationProvider = context.CompilationProvider;

        IncrementalValuesProvider<(InterfaceDeclarationSyntax Interface, AttributeSyntax Attribute, Compilation Compilation)> interfaceDeclarations = 
            context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => SyntaxTargetIdentifier.IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => SemanticTargetIdentifier.GetSemanticTargetsForGeneration(ctx))
                .Where(static m => m.Interface is not null && m.Attributes.Count > 0)
                .SelectMany((target, _) => 
                    target.Attributes.Select(attr => (target.Interface, Attribute: attr)))
                .Combine(compilationProvider)
                .Select((tuple, _) => (
                    tuple.Left.Interface,
                    tuple.Left.Attribute,
                    tuple.Right));

        // Register the source output
        context.RegisterSourceOutput(interfaceDeclarations,
            (spc, source) => new HandlerAttributeExecutor().Execute(source.Interface, source.Attribute, source.Compilation, spc));
    }
} 