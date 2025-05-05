using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.UseHandlerAttributes.Utils;

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
    public static List<ITypeSymbol> GetBaseGenericTypeArguments(ITypeSymbol handlerTypeSymbol)
    {
        // Find the EntityCommandHandler or EntityQueryHandler base class and get its type arguments
        var current = handlerTypeSymbol;
        
        while (current != null)
        {
            if (current is INamedTypeSymbol namedType && 
                (current.Name == "EntityCommandHandler" || current.Name == "EntityQueryHandler"))
            {
                return namedType.TypeArguments.ToList();
            }
            
            current = current.BaseType;
        }
        
        return new List<ITypeSymbol>();
    }


    /// <summary>
    /// Gets the generic type arguments of a handler type.
    /// </summary>
    /// <param name="handlerTypeSymbol">The handler type symbol</param>
    /// <returns>The list of generic type arguments</returns>
    public static List<ITypeSymbol> GetGenericTypeArguments(ITypeSymbol handlerTypeSymbol)
    {
        // Find the EntityCommandHandler or EntityQueryHandler base class and get its type arguments
        var current = handlerTypeSymbol;
    
        if (current is INamedTypeSymbol namedType)
        {
            return namedType.TypeArguments.ToList();
        }
        
        return new List<ITypeSymbol>();
    }

    /// <summary>
    /// Gets the generic constraints for a handler type.
    /// </summary>
    /// <param name="handlerTypeSymbol">The handler type symbol</param>
    /// <returns>A list of generic constraints</returns>
    public static List<GenericConstraint> GetGenericConstraints(ITypeSymbol handlerTypeSymbol)
    {
        var constraints = new List<GenericConstraint>();
        
        // Get the generic type parameters of the handler type
        if (!(handlerTypeSymbol is INamedTypeSymbol namedTypeSymbol))
        {
            return constraints;
        }
        
        // Get the generic types for the handler
        var genericTypes = GetGenericTypeArguments(handlerTypeSymbol);
        
        // Get the generic types for the base command/query handler
        var baseGenericTypes = GetBaseGenericTypeArguments(handlerTypeSymbol);
        
        // Process each type parameter
        foreach (var typeParam in namedTypeSymbol.TypeParameters)
        {
            // Find the corresponding type in the genericTypes list
            var typeIndex = -1;
            for (int i = 0; i < genericTypes.Count; i++)
            {
                if (genericTypes[i].Name == typeParam.Name)
                {
                    typeIndex = i;
                    break;
                }
            }
            
            // Determine the constraint type based on position in baseGenericTypes
            GenericConstraintType constraintType;
            if (typeIndex == -1)
            {
                constraintType = GenericConstraintType.None;
            }
            else if (typeIndex == 0)
            {
                constraintType = GenericConstraintType.Class;
            }
            else if (typeIndex == 1)
            {
                constraintType = baseGenericTypes.Count >= 3 
                    ? GenericConstraintType.Input 
                    : GenericConstraintType.Output;
            }
            else if (typeIndex == 2)
            {
                constraintType = GenericConstraintType.Output;
            }
            else
            {
                // Skip if we can't determine the constraint type
                continue;
            }
            
            // Add reference/value type constraints
            if (typeParam.HasReferenceTypeConstraint)
            {
                constraints.Add(new GenericConstraint
                {
                    Type = constraintType,
                    Constraint = "class"
                });
            }
            else if (typeParam.HasValueTypeConstraint)
            {
                constraints.Add(new GenericConstraint
                {
                    Type = constraintType,
                    Constraint = "struct"
                });
            }
            
            // Add unmanaged constraint
            if (typeParam.HasUnmanagedTypeConstraint)
            {
                constraints.Add(new GenericConstraint
                {
                    Type = constraintType,
                    Constraint = "unmanaged"
                });
            }
            
            // Add notnull constraint
            if (typeParam.HasNotNullConstraint)
            {
                constraints.Add(new GenericConstraint
                {
                    Type = constraintType,
                    Constraint = "notnull"
                });
            }
            
            // Add interface and class constraints
            foreach (var constraintTypeSymbol in typeParam.ConstraintTypes)
            {
                constraints.Add(new GenericConstraint
                {
                    Type = constraintType,
                    Constraint = constraintTypeSymbol.ToDisplayString()
                });
            }
            
            // Add constructor constraint
            if (typeParam.HasConstructorConstraint)
            {
                constraints.Add(new GenericConstraint
                {
                    Type = constraintType,
                    Constraint = "new()"
                });
            }
        }
        
        return constraints;
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

public enum GenericConstraintType {
    None,
    Class,
    Input,
    Output
}

public record GenericConstraint {
    public GenericConstraintType Type { get; set; }
    public string Constraint { get; set; } = string.Empty;
}