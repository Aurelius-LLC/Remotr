using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.CqrsCollection.Validators;

/// <summary>
/// Validates interface declarations for code generation.
/// </summary>
public class InterfaceValidator
{
    /// <summary>
    /// Checks if the interface implements ITransactionManagerGrain.
    /// </summary>
    /// <param name="interfaceDeclaration">The interface declaration to check</param>
    /// <param name="compilation">The current compilation</param>
    /// <returns>True if the interface implements ITransactionManagerGrain</returns>
    public bool ImplementsITransactionManagerGrain(InterfaceDeclarationSyntax interfaceDeclaration, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);
        var interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration) as INamedTypeSymbol;
        
        if (interfaceSymbol == null)
            return false;

        var iTransactionManagerGrainSymbol = compilation.GetTypeByMetadataName("Remotr.ITransactionManagerGrain");
        
        if (iTransactionManagerGrainSymbol == null)
            return false;

        return interfaceSymbol.AllInterfaces.Contains(iTransactionManagerGrainSymbol, SymbolEqualityComparer.Default);
    }
} 