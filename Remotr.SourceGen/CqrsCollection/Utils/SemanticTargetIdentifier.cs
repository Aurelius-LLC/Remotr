using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Remotr.SourceGen.CqrsCollection;

/// <summary>
/// Identifies semantic targets for code generation.
/// </summary>
public static class SemanticTargetIdentifier
{
    /// <summary>
    /// Gets the semantic model targets for code generation.
    /// </summary>
    /// <param name="context">The generator syntax context</param>
    /// <returns>The interface declaration and all CqrsCollection attributes on it</returns>
    public static (InterfaceDeclarationSyntax Interface, List<AttributeSyntax> Attributes) GetSemanticTargetsForGeneration(GeneratorSyntaxContext context)
    {
        var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;
        var cqrsCollectionAttributes = new List<AttributeSyntax>();
        
        foreach (var attributeList in interfaceDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Remotr.CqrsCollectionAttribute")
                {
                    cqrsCollectionAttributes.Add(attribute);
                }
            }
        }

        return (interfaceDeclaration, cqrsCollectionAttributes);
    }
} 