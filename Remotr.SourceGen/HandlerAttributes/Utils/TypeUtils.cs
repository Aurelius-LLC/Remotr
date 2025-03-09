using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.HandlerAttributes.Utils;

/// <summary>
/// Utility methods for working with types.
/// </summary>
public static class TypeUtils
{
    /// <summary>
    /// Gets the generic type arguments of a handler type.
    /// </summary>
    /// <param name="handlerTypeSymbol">The handler type symbol</param>
    /// <returns>The list of generic type arguments</returns>
    public static List<ITypeSymbol> GetGenericTypeArguments(ITypeSymbol handlerTypeSymbol)
    {
        // Find the StatefulCommandHandler or StatefulQueryHandler base class and get its type arguments
        var current = handlerTypeSymbol;
        
        while (current != null)
        {
            if (current is INamedTypeSymbol namedType && 
                (current.Name == "StatefulCommandHandler" || current.Name == "StatefulQueryHandler"))
            {
                return namedType.TypeArguments.ToList();
            }
            
            current = current.BaseType;
        }
        
        return new List<ITypeSymbol>();
    }

    /// <summary>
    /// Gets the namespace of an interface declaration.
    /// </summary>
    /// <param name="interfaceDeclaration">The interface declaration</param>
    /// <returns>The namespace of the interface declaration</returns>
    public static string GetNamespace(InterfaceDeclarationSyntax interfaceDeclaration)
    {
        // Get the namespace from the syntax tree
        var namespaceDecl = interfaceDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        return namespaceDecl?.Name.ToString() ?? string.Empty;
    }
} 