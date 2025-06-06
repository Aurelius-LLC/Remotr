using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Remotr.SourceGen.UseHandlerAttributes.Validators;

/// <summary>
/// Validates interface declarations for code generation.
/// </summary>
public class InterfaceValidator
{
    /// <summary>
    /// Checks if an interface implements IAggregateRoot.
    /// </summary>
    /// <param name="interfaceDeclaration">The interface declaration to check</param>
    /// <param name="compilation">The current compilation</param>
    /// <returns>True if the interface implements IAggregateRoot</returns>
    public bool ImplementsIAggregateRoot(InterfaceDeclarationSyntax interfaceDeclaration, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);
        var interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration) as INamedTypeSymbol;
        
        if (interfaceSymbol == null)
            return false;

        // Try different namespace possibilities
        string[] possibleNamespaces = new[] { "Remotr", "Remotr.Interfaces" };
        
        foreach (var ns in possibleNamespaces)
        {
            var aggregateRootGrainSymbol = compilation.GetTypeByMetadataName($"{ns}.IAggregateRoot");
            
            if (aggregateRootGrainSymbol != null && 
                interfaceSymbol.AllInterfaces.Any(i => i.Equals(aggregateRootGrainSymbol, SymbolEqualityComparer.Default)))
            {
                return true;
            }
        }
        
        return false;
    }
} 