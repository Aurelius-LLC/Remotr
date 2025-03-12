using Microsoft.CodeAnalysis;

namespace Remotr.SourceGen.UseHandlerAttributes.Validators;

/// <summary>
/// Validates handler types for code generation.
/// </summary>
public class HandlerTypeValidator
{
    /// <summary>
    /// Checks if the handler type is valid (extends StatefulCommandHandler or StatefulQueryHandler).
    /// </summary>
    /// <param name="isCommandHandler">Whether this is validating a command handler or query handler</param>
    /// <param name="handlerTypeSymbol">The handler type symbol to check</param>
    /// <param name="compilation">The current compilation</param>
    /// <returns>True if the handler type is valid</returns>
    public bool IsValidHandlerType(bool isCommandHandler, ITypeSymbol handlerTypeSymbol, Compilation compilation)
    {
        // Try different namespace possibilities
        string[] possibleNamespaces = new[] { "Remotr" };
        INamedTypeSymbol? statefulCommandHandlerSymbol = null;
        INamedTypeSymbol? statefulQueryHandlerSymbol = null;

        foreach (var ns in possibleNamespaces)
        {
            string prefix = string.IsNullOrEmpty(ns) ? "" : ns + ".";
            
            // Try different generic versions
            statefulCommandHandlerSymbol = 
                compilation.GetTypeByMetadataName($"{prefix}StatefulCommandHandler`1") ??
                compilation.GetTypeByMetadataName($"{prefix}StatefulCommandHandler`2") ??
                compilation.GetTypeByMetadataName($"{prefix}StatefulCommandHandler`3");
            
            statefulQueryHandlerSymbol = 
                compilation.GetTypeByMetadataName($"{prefix}StatefulQueryHandler`1") ??
                compilation.GetTypeByMetadataName($"{prefix}StatefulQueryHandler`2") ??
                compilation.GetTypeByMetadataName($"{prefix}StatefulQueryHandler`3");

            // If we found either type, break out of the loop
            if (statefulCommandHandlerSymbol != null || statefulQueryHandlerSymbol != null)
                break;
        }

        // Check inheritance directly by looking at base types
        var current = handlerTypeSymbol.BaseType;
        while (current != null)
        {
            var currentName = current.Name;
            
            // Check if the name contains the expected string (simplified check)
            if ((currentName == "StatefulCommandHandler" && isCommandHandler) || (currentName == "StatefulQueryHandler" && !isCommandHandler))
                return true;
                
            current = current.BaseType;
        }
        
        // Fallback to original check if direct name check didn't succeed
        if (statefulCommandHandlerSymbol == null && statefulQueryHandlerSymbol == null)
            return false;

        bool isCommandHandlerType = statefulCommandHandlerSymbol != null && 
                              IsSubtypeOfOpenGeneric(handlerTypeSymbol, statefulCommandHandlerSymbol);
                              
        bool isQueryHandlerType = statefulQueryHandlerSymbol != null && 
                            IsSubtypeOfOpenGeneric(handlerTypeSymbol, statefulQueryHandlerSymbol);
                            
        return (isCommandHandlerType && isCommandHandler) || (isQueryHandlerType && !isCommandHandler);
    }

    /// <summary>
    /// Checks if a type is a subtype of an open generic type.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <param name="openGenericType">The open generic type</param>
    /// <returns>True if the type is a subtype of the open generic type</returns>
    public bool IsSubtypeOfOpenGeneric(ITypeSymbol type, INamedTypeSymbol? openGenericType)
    {
        if (openGenericType == null)
            return false;

        // Quick name check first (faster than full symbol checking)
        bool isMatchingName = false;
        var originalDefName = openGenericType.OriginalDefinition.Name;
        
        var current = type;
        while (current != null)
        {
            if (current.Name.Contains(originalDefName.Split('`')[0]))
            {
                isMatchingName = true;
                break;
            }
            
            current = current.BaseType;
        }
        
        if (!isMatchingName)
            return false;

        // Now do the full check if name matched
        current = type;
        while (current != null)
        {
            if (current is INamedTypeSymbol namedType && 
                current.OriginalDefinition.Equals(openGenericType.OriginalDefinition, SymbolEqualityComparer.Default))
                return true;

            // Check if any of the interfaces it implements are the open generic type
            foreach (var iface in current.AllInterfaces)
            {
                if (iface.OriginalDefinition.Equals(openGenericType.OriginalDefinition, SymbolEqualityComparer.Default))
                    return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Checks if the handler type is a command handler.
    /// </summary>
    /// <param name="handlerTypeSymbol">The handler type symbol to check</param>
    /// <returns>True if the handler type is a command handler</returns>
    public bool IsCommandHandler(ITypeSymbol handlerTypeSymbol)
    {
        // Check if the base type's name contains "CommandHandler"
        var current = handlerTypeSymbol;
        while (current != null)
        {
            if (current.Name.Contains("CommandHandler"))
                return true;
            
            current = current.BaseType;
        }
        
        return false;
    }
} 