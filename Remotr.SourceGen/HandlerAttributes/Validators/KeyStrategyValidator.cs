using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Collections.Generic;

namespace Remotr.SourceGen.HandlerAttributes.Validators;

/// <summary>
/// Validates key strategy configurations.
/// </summary>
public class KeyStrategyValidator
{
    /// <summary>
    /// Validates that at most one key strategy is specified.
    /// </summary>
    /// <param name="fixed">The fixed key value.</param>
    /// <param name="find">The find method name.</param>
    /// <param name="usePrimaryKey">Whether to use the primary key.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="location">The location for diagnostic reporting.</param>
    /// <returns>True if the validation passes, false otherwise.</returns>
    public bool ValidateKeyStrategyOptions(
        string? @fixed,
        string? find,
        bool usePrimaryKey,
        SourceProductionContext context,
        Location location)
    {
        int optionsSpecified = 0;
        
        if (!string.IsNullOrEmpty(@fixed)) optionsSpecified++;
        if (!string.IsNullOrEmpty(find)) optionsSpecified++;
        if (usePrimaryKey) optionsSpecified++;

        if (optionsSpecified > 1)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR010",
                        "Multiple key strategy options specified",
                        "Only one key strategy option (fixed, find, or usePrimaryKey) can be specified at a time",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    location));
            return false;
        }

        // Add diagnostic for validation result
        context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "INFO006",
                    "Key strategy validation",
                    "Key strategy validation: fixedKey='{0}', findMethod='{1}', usePrimaryKey={2}, optionsSpecified={3}",
                    "Remotr",
                    DiagnosticSeverity.Info,
                    isEnabledByDefault: true),
                location,
                @fixed ?? "null",
                find ?? "null",
                usePrimaryKey,
                optionsSpecified));

        return true;
    }

    /// <summary>
    /// Validates that a static method exists and has the correct signature.
    /// </summary>
    /// <param name="methodName">The method name to find.</param>
    /// <param name="interfaceDeclaration">The interface declaration.</param>
    /// <param name="hasInput">Whether the handler has input.</param>
    /// <param name="inputType">The input type if the handler has input.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The source production context.</param>
    /// <returns>True if the validation passes, false otherwise.</returns>
    public bool ValidateStaticMethod(
        string methodName,
        InterfaceDeclarationSyntax interfaceDeclaration,
        bool hasInput,
        string? inputType,
        Compilation compilation,
        SourceProductionContext context)
    {
        var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);
        var interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration) as INamedTypeSymbol;
        
        if (interfaceSymbol == null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR011",
                        "Interface symbol not found",
                        "Could not find symbol for interface '{0}'",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    interfaceDeclaration.Identifier.Text));
            return false;
        }

        // Look for the static method
        var members = interfaceSymbol.GetMembers();
        var methodSymbol = members.FirstOrDefault(m => 
            m.Kind == SymbolKind.Method && 
            m.IsStatic && 
            m.Name == methodName) as IMethodSymbol;

        if (methodSymbol == null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR012",
                        "Static method not found",
                        "Could not find static method '{0}' on interface '{1}'",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    methodName,
                    interfaceDeclaration.Identifier.Text));
            return false;
        }

        // Check if the method returns a string
        if (methodSymbol.ReturnType.SpecialType != SpecialType.System_String)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR013",
                        "Invalid method return type",
                        "The static method '{0}' must return a string",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    methodName));
            return false;
        }

        // Check if the method signature matches the handler input requirements
        if (hasInput && methodSymbol.Parameters.Length == 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR014",
                        "Method parameter mismatch",
                        "The static method '{0}' must accept a parameter because the handler has an input",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    methodName));
            return false;
        }
        else if (!hasInput && methodSymbol.Parameters.Length > 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR015",
                        "Method parameter mismatch",
                        "The static method '{0}' must not have parameters because the handler does not have an input",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    methodName));
            return false;
        }
        else if (hasInput && methodSymbol.Parameters.Length > 0)
        {
            // Check if parameter type matches the input type
            ITypeSymbol? inputTypeSymbol = null;
            
            // Handle special case for primitive types
            if (TryGetPrimitiveTypeSymbol(compilation, inputType!, out var primitiveTypeSymbol))
            {
                inputTypeSymbol = primitiveTypeSymbol;
            }
            else if (TryResolveComplexType(compilation, inputType!, out var complexTypeSymbol))
            {
                // Handle complex types like generics, arrays, etc.
                inputTypeSymbol = complexTypeSymbol;
            }
            else
            {
                // Try to get the type by metadata name for non-primitive types
                inputTypeSymbol = compilation.GetTypeByMetadataName(inputType!);
                
                // If that fails, try to get it by unqualified name
                if (inputTypeSymbol == null)
                {
                    // Try to find the type in the global namespace, which should include
                    // types from using directives in the original source
                    inputTypeSymbol = FindTypeBySimpleName(compilation, inputType!);
                }
            }
            
            if (inputTypeSymbol == null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "REMOTR016",
                            "Input type not found",
                            "Could not find type '{0}' for input parameter",
                            "Remotr",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        interfaceDeclaration.GetLocation(),
                        inputType));
                return false;
            }

            var parameterTypeSymbol = methodSymbol.Parameters[0].Type;
            if (!AreTypesEquivalent(parameterTypeSymbol, inputTypeSymbol, compilation))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "REMOTR017",
                            "Parameter type mismatch",
                            "The type of the method parameter '{0}' does not match the handler input type '{1}'",
                            "Remotr",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        interfaceDeclaration.GetLocation(),
                        parameterTypeSymbol.ToDisplayString(),
                        inputTypeSymbol.ToDisplayString()));
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if two types are equivalent, considering various type representations.
    /// </summary>
    /// <param name="type1">The first type.</param>
    /// <param name="type2">The second type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>True if the types are equivalent, false otherwise.</returns>
    private bool AreTypesEquivalent(ITypeSymbol type1, ITypeSymbol type2, Compilation compilation)
    {
        // Try direct equality first
        if (SymbolEqualityComparer.Default.Equals(type1, type2))
        {
            return true;
        }
        
        // Handle nullable types
        if (type1 is INamedTypeSymbol namedType1 && namedType1.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T &&
            namedType1.TypeArguments.Length == 1)
        {
            if (SymbolEqualityComparer.Default.Equals(namedType1.TypeArguments[0], type2))
            {
                return true;
            }
        }
        else if (type2 is INamedTypeSymbol namedType2 && namedType2.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T &&
                 namedType2.TypeArguments.Length == 1)
        {
            if (SymbolEqualityComparer.Default.Equals(type1, namedType2.TypeArguments[0]))
            {
                return true;
            }
        }
        
        // Compare display strings as a last resort
        return type1.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == 
               type2.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    /// <summary>
    /// Tries to get a type symbol for common primitive types.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="typeName">The type name (e.g. "int", "string").</param>
    /// <param name="typeSymbol">The resulting type symbol if successful.</param>
    /// <returns>True if a primitive type was found, false otherwise.</returns>
    private bool TryGetPrimitiveTypeSymbol(Compilation compilation, string typeName, out ITypeSymbol? typeSymbol)
    {
        typeSymbol = null;
        
        // Map of C# type keywords to their fully qualified metadata names
        var primitiveTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "bool", "System.Boolean" },
            { "byte", "System.Byte" },
            { "sbyte", "System.SByte" },
            { "char", "System.Char" },
            { "decimal", "System.Decimal" },
            { "double", "System.Double" },
            { "float", "System.Single" },
            { "int", "System.Int32" },
            { "uint", "System.UInt32" },
            { "long", "System.Int64" },
            { "ulong", "System.UInt64" },
            { "short", "System.Int16" },
            { "ushort", "System.UInt16" },
            { "object", "System.Object" },
            { "string", "System.String" },
            { "dynamic", "System.Object" }
        };
        
        if (primitiveTypeMap.TryGetValue(typeName, out var metadataName))
        {
            typeSymbol = compilation.GetTypeByMetadataName(metadataName);
            return typeSymbol != null;
        }
        
        return false;
    }

    /// <summary>
    /// Attempts to find a type by its simple name in the compilation.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="simpleName">The simple name of the type.</param>
    /// <returns>The type symbol if found, null otherwise.</returns>
    private ITypeSymbol? FindTypeBySimpleName(Compilation compilation, string simpleName)
    {
        // Look for the type in all namespaces
        foreach (var namespaceSymbol in GetAllNamespaces(compilation.GlobalNamespace))
        {
            foreach (var typeSymbol in namespaceSymbol.GetTypeMembers())
            {
                if (typeSymbol.Name == simpleName)
                {
                    return typeSymbol;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Gets all namespaces in the compilation, including nested ones.
    /// </summary>
    /// <param name="rootNamespace">The root namespace.</param>
    /// <returns>All namespaces.</returns>
    private IEnumerable<INamespaceSymbol> GetAllNamespaces(INamespaceSymbol rootNamespace)
    {
        yield return rootNamespace;
        
        foreach (var nestedNamespace in rootNamespace.GetNamespaceMembers())
        {
            foreach (var ns in GetAllNamespaces(nestedNamespace))
            {
                yield return ns;
            }
        }
    }
    
    /// <summary>
    /// Tries to resolve a complex type like a generic or array type.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="typeName">The type name.</param>
    /// <param name="typeSymbol">The resulting type symbol if successful.</param>
    /// <returns>True if a complex type was resolved, false otherwise.</returns>
    private bool TryResolveComplexType(Compilation compilation, string typeName, out ITypeSymbol? typeSymbol)
    {
        typeSymbol = null;
        
        // Check if it's an array type
        if (typeName.EndsWith("[]"))
        {
            var elementTypeName = typeName.Substring(0, typeName.Length - 2);
            
            ITypeSymbol? elementType = null;
            if (TryGetPrimitiveTypeSymbol(compilation, elementTypeName, out var primitiveElementType))
            {
                elementType = primitiveElementType;
            }
            else
            {
                elementType = compilation.GetTypeByMetadataName(elementTypeName);
                
                if (elementType == null)
                {
                    elementType = FindTypeBySimpleName(compilation, elementTypeName);
                }
            }
            
            if (elementType != null)
            {
                typeSymbol = compilation.CreateArrayTypeSymbol(elementType);
                return true;
            }
        }
        
        // This could be expanded to handle generic types, but that's more complex
        // and would require parsing the type name to extract the generic arguments
        
        return false;
    }
} 