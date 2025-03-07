using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.HandlerAttributes.Validators;

/// <summary>
/// Validates attribute arguments for code generation.
/// </summary>
public class AttributeValidator
{
    /// <summary>
    /// Gets the attribute type name.
    /// </summary>
    /// <param name="attribute">The attribute to check</param>
    /// <param name="compilation">The current compilation</param>
    /// <returns>The attribute type name</returns>
    public string GetAttributeTypeName(AttributeSyntax attribute, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(attribute.SyntaxTree);
        if (semanticModel.GetSymbolInfo(attribute).Symbol is IMethodSymbol attributeSymbol)
        {
            return attributeSymbol.ContainingType.Name;
        }
        return string.Empty;
    }

    /// <summary>
    /// Validates the attribute arguments and extracts the handler type and alias.
    /// </summary>
    /// <param name="attribute">The attribute to validate</param>
    /// <param name="interfaceDeclaration">The interface declaration</param>
    /// <param name="compilation">The current compilation</param>
    /// <param name="context">The source production context</param>
    /// <param name="handlerTypeSymbol">The extracted handler type symbol</param>
    /// <param name="alias">The extracted alias</param>
    /// <returns>True if the attribute arguments are valid</returns>
    public bool ValidateAttributeArguments(
        AttributeSyntax attribute, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        Compilation compilation,
        SourceProductionContext context,
        out ITypeSymbol handlerTypeSymbol,
        out string alias)
    {
        handlerTypeSymbol = null;
        alias = null;

        // Get attribute name for error messages
        string attributeName = GetAttributeTypeName(attribute, compilation);
        if (string.IsNullOrEmpty(attributeName))
        {
            attributeName = "attribute";
        }

        // Check if the attribute has the correct number of arguments
        if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count != 2)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR002",
                        $"{attributeName} must have two arguments",
                        "The {0} attribute on interface '{1}' must have two arguments: Type handlerType and string alias",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    attribute.GetLocation(),
                    attributeName,
                    interfaceDeclaration.Identifier.Text));
            return false;
        }

        // Get the handler type from the attribute
        var handlerTypeArgument = attribute.ArgumentList.Arguments[0];
        var aliasArgument = attribute.ArgumentList.Arguments[1];

        // Validate handler type argument
        if (handlerTypeArgument.Expression is not TypeOfExpressionSyntax typeOfExpression)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR003",
                        "First argument must be a type",
                        "The first argument of {0} attribute on interface '{1}' must be a type expression (typeof(...))",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    handlerTypeArgument.GetLocation(),
                    attributeName,
                    interfaceDeclaration.Identifier.Text));
            return false;
        }

        // Validate alias argument
        if (aliasArgument.Expression is not LiteralExpressionSyntax aliasLiteral || 
            aliasLiteral.Kind() != SyntaxKind.StringLiteralExpression)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR004",
                        "Second argument must be a string",
                        "The second argument of {0} attribute on interface '{1}' must be a string literal",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    aliasArgument.GetLocation(),
                    attributeName,
                    interfaceDeclaration.Identifier.Text));
            return false;
        }

        // Get the handler type symbol
        var handlerTypeInfo = compilation.GetSemanticModel(typeOfExpression.SyntaxTree)
            .GetTypeInfo(typeOfExpression.Type);

        if (handlerTypeInfo.Type == null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR005",
                        "Handler type not found",
                        "The handler type in {0} attribute on interface '{1}' could not be resolved",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    typeOfExpression.GetLocation(),
                    attributeName,
                    interfaceDeclaration.Identifier.Text));
            return false;
        }

        handlerTypeSymbol = handlerTypeInfo.Type;
        alias = aliasLiteral.Token.ValueText;

        // Ensure the alias is different from the handler type name
        if (handlerTypeSymbol.Name == alias)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR007",
                        "Alias must be different from handler type name",
                        "The alias '{0}' in {1} attribute must be different from the handler type name",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    aliasArgument.GetLocation(),
                    alias,
                    attributeName));
            return false;
        }

        return true;
    }
} 