using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

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
            var inputTypeSymbol = compilation.GetTypeByMetadataName(inputType!);
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
            if (!SymbolEqualityComparer.Default.Equals(parameterTypeSymbol, inputTypeSymbol))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "REMOTR017",
                            "Parameter type mismatch",
                            "The static method '{0}' parameter type '{1}' does not match the handler input type '{2}'",
                            "Remotr",
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true),
                        interfaceDeclaration.GetLocation(),
                        methodName,
                        parameterTypeSymbol.Name,
                        inputTypeSymbol.Name));
                return false;
            }
        }

        return true;
    }
} 