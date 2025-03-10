using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotr.SourceGen.HandlerAttributes.Utils;

namespace Remotr.SourceGen.HandlerAttributes.Validators;

/// <summary>
/// Validates attribute arguments for code generation.
/// </summary>
public class AttributeValidator
{
    private readonly KeyStrategyValidator _keyStrategyValidator;

    public AttributeValidator()
    {
        _keyStrategyValidator = new KeyStrategyValidator();
    }

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
    /// Validates the attribute arguments and extracts the handler type, alias, and key strategy options.
    /// </summary>
    /// <param name="attribute">The attribute to validate</param>
    /// <param name="interfaceDeclaration">The interface declaration</param>
    /// <param name="compilation">The current compilation</param>
    /// <param name="context">The source production context</param>
    /// <param name="handlerTypeSymbol">The extracted handler type symbol</param>
    /// <param name="alias">The extracted alias</param>
    /// <param name="fixedKey">The fixed key, if specified</param>
    /// <param name="findMethod">The find method name, if specified</param>
    /// <param name="usePrimaryKey">Whether to use the primary key</param>
    /// <returns>True if the attribute arguments are valid</returns>
    public bool ValidateAttributeArguments(
        AttributeSyntax attribute, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        Compilation compilation,
        SourceProductionContext context,
        out ITypeSymbol? handlerTypeSymbol,
        out string? alias,
        out string? fixedKey,
        out string? findMethod,
        out bool usePrimaryKey)
    {
        // Initialize out parameters
        handlerTypeSymbol = null;
        alias = null;
        fixedKey = null;
        findMethod = null;
        usePrimaryKey = false;

        // Get attribute name for error messages
        string attributeName = GetAttributeTypeName(attribute, compilation);
        if (string.IsNullOrEmpty(attributeName))
        {
            attributeName = "attribute";
        }

        // Validate basic attribute structure
        if (!ValidateAttributeStructure(attribute, interfaceDeclaration, attributeName, context))
        {
            return false;
        }

        // Extract and validate handler type and alias
        if (!ExtractAndValidateHandlerTypeAndAlias(
            attribute, 
            interfaceDeclaration, 
            compilation, 
            attributeName,
            context, 
            out handlerTypeSymbol, 
            out alias))
        {
            return false;
        }

        // Extract key strategy options from named arguments
        ExtractKeyStrategyOptions(
            attribute, 
            compilation, 
            context, 
            out fixedKey, 
            out findMethod, 
            out usePrimaryKey);

        // Use default if no key strategy is specified
        if (string.IsNullOrEmpty(fixedKey) && string.IsNullOrEmpty(findMethod) && !usePrimaryKey)
        {
            usePrimaryKey = true;
        }

        // Validate key strategy options
        if (!_keyStrategyValidator.ValidateKeyStrategyOptions(fixedKey, findMethod, usePrimaryKey, context, attribute.GetLocation()))
        {
            return false;
        }

        // If using find method, validate the static method
        if (!string.IsNullOrEmpty(findMethod) && !ValidateFindMethod(findMethod, interfaceDeclaration, handlerTypeSymbol, compilation, context))
        {
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Validates the basic structure of the attribute.
    /// </summary>
    /// <param name="attribute">The attribute syntax.</param>
    /// <param name="interfaceDeclaration">The interface declaration.</param>
    /// <param name="attributeName">The name of the attribute for error messages.</param>
    /// <param name="context">The source production context.</param>
    /// <returns>True if the attribute structure is valid, false otherwise.</returns>
    private bool ValidateAttributeStructure(
        AttributeSyntax attribute, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        string attributeName,
        SourceProductionContext context)
    {
        // Check if the attribute has at least the required arguments
        if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count < 2)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR002",
                        $"{attributeName} must have at least two arguments: Type handlerType and string alias",
                        "The {0} attribute on interface '{1}' must have at least two arguments: Type handlerType and string alias",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    attribute.GetLocation(),
                    attributeName,
                    interfaceDeclaration.Identifier.Text));
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Extracts and validates the handler type and alias from the attribute.
    /// </summary>
    /// <param name="attribute">The attribute syntax.</param>
    /// <param name="interfaceDeclaration">The interface declaration.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="attributeName">The name of the attribute for error messages.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="handlerTypeSymbol">The handler type symbol.</param>
    /// <param name="alias">The alias.</param>
    /// <returns>True if the handler type and alias are valid, false otherwise.</returns>
    private bool ExtractAndValidateHandlerTypeAndAlias(
        AttributeSyntax attribute, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        Compilation compilation,
        string attributeName,
        SourceProductionContext context,
        out ITypeSymbol? handlerTypeSymbol,
        out string? alias)
    {
        handlerTypeSymbol = null;
        alias = null;
        
        // Get the handler type from the attribute
        var handlerTypeArgument = attribute.ArgumentList.Arguments[0];
        var aliasArgument = attribute.ArgumentList.Arguments[1];

        // Validate handler type argument
        if (!ValidateHandlerTypeArgument(handlerTypeArgument, interfaceDeclaration, attributeName, context))
        {
            return false;
        }

        // Validate alias argument
        if (!ValidateAliasArgument(aliasArgument, interfaceDeclaration, attributeName, context))
        {
            return false;
        }

        // Get the handler type symbol
        var typeOfExpression = (TypeOfExpressionSyntax)handlerTypeArgument.Expression;
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
        var aliasLiteral = (LiteralExpressionSyntax)aliasArgument.Expression;
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

    /// <summary>
    /// Validates the handler type argument.
    /// </summary>
    /// <param name="handlerTypeArgument">The handler type argument.</param>
    /// <param name="interfaceDeclaration">The interface declaration.</param>
    /// <param name="attributeName">The name of the attribute for error messages.</param>
    /// <param name="context">The source production context.</param>
    /// <returns>True if the handler type argument is valid, false otherwise.</returns>
    private bool ValidateHandlerTypeArgument(
        AttributeArgumentSyntax handlerTypeArgument, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        string attributeName,
        SourceProductionContext context)
    {
        if (handlerTypeArgument.Expression is not TypeOfExpressionSyntax)
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
        
        return true;
    }

    /// <summary>
    /// Validates the alias argument.
    /// </summary>
    /// <param name="aliasArgument">The alias argument.</param>
    /// <param name="interfaceDeclaration">The interface declaration.</param>
    /// <param name="attributeName">The name of the attribute for error messages.</param>
    /// <param name="context">The source production context.</param>
    /// <returns>True if the alias argument is valid, false otherwise.</returns>
    private bool ValidateAliasArgument(
        AttributeArgumentSyntax aliasArgument, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        string attributeName,
        SourceProductionContext context)
    {
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
        
        return true;
    }

    /// <summary>
    /// Extracts key strategy options from the attribute arguments.
    /// </summary>
    /// <param name="attribute">The attribute syntax.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="fixedKey">The fixed key value.</param>
    /// <param name="findMethod">The find method name.</param>
    /// <param name="usePrimaryKey">Whether to use the primary key.</param>
    private void ExtractKeyStrategyOptions(
        AttributeSyntax attribute, 
        Compilation compilation,
        SourceProductionContext context,
        out string? fixedKey,
        out string? findMethod,
        out bool usePrimaryKey)
    {
        fixedKey = null;
        findMethod = null;
        usePrimaryKey = false;
        
        // Get the semantic model for the attribute
        var semanticModel = compilation.GetSemanticModel(attribute.SyntaxTree);
        
        // Get the attribute constructor symbol
        if (semanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeConstructor)
        {
            return;
        }

        // Extract key strategy options from named arguments
        foreach (var arg in attribute.ArgumentList.Arguments.Skip(2))
        {
            // Get parameter name
            string paramName = GetParameterName(arg, semanticModel, attributeConstructor, context);
            
            if (string.IsNullOrEmpty(paramName))
            {
                continue;
            }

            // Process the parameter based on its name
            ProcessParameter(paramName, arg, compilation, context, ref fixedKey, ref findMethod, ref usePrimaryKey);
        }
    }

    /// <summary>
    /// Gets the parameter name from an attribute argument.
    /// </summary>
    /// <param name="arg">The attribute argument.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="attributeConstructor">The attribute constructor symbol.</param>
    /// <param name="context">The source production context for reporting diagnostics.</param>
    /// <returns>The parameter name, or an empty string if it couldn't be determined.</returns>
    private string GetParameterName(
        AttributeArgumentSyntax arg, 
        SemanticModel semanticModel, 
        IMethodSymbol attributeConstructor,
        SourceProductionContext context)
    {

        // If the argument has an explicit name, use that first (highest priority)
        if (arg.NameColon != null)
        {
            return arg.NameColon.Name.Identifier.Text;
        }
        
        // Try to get the parameter name from the semantic model
        var symbolInfo = semanticModel.GetSymbolInfo(arg.Expression);
        var parameterSymbol = symbolInfo.Symbol as IParameterSymbol;

        if (parameterSymbol != null && !string.IsNullOrEmpty(parameterSymbol.Name))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR104",
                        "Found semantic name",
                        "Found semantic name: '{0}'",
                        "Remotr",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    arg.GetLocation(),
                    parameterSymbol.Name));
            return parameterSymbol.Name;
        }

        // Try to get the parameter name by matching the argument type with constructor parameters
        if (arg.Expression is LiteralExpressionSyntax literal)
        {
            var matchingParams = attributeConstructor.Parameters
                .Where(p => IsTypeMatch(p.Type, literal))
                .ToList();

            if (matchingParams.Count == 1)
            {
                var name = matchingParams[0].Name;
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "REMOTR105",
                            "Found type match name",
                            "Found type match name: '{0}'",
                            "Remotr",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true),
                        arg.GetLocation(),
                        name));
                return name;
            }
        }
        
        context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "REMOTR106",
                    "No name found",
                    "Could not determine parameter name",
                    "Remotr",
                    DiagnosticSeverity.Warning,
                    isEnabledByDefault: true),
                arg.GetLocation()));
        return string.Empty;
    }

    private bool IsTypeMatch(ITypeSymbol parameterType, LiteralExpressionSyntax literal)
    {
        // Match string literals with string parameter type
        if (literal.Kind() == SyntaxKind.StringLiteralExpression && 
            parameterType.SpecialType == SpecialType.System_String)
        {
            return true;
        }

        // Match boolean literals with bool parameter type
        if ((literal.Kind() == SyntaxKind.TrueLiteralExpression || 
             literal.Kind() == SyntaxKind.FalseLiteralExpression) && 
            parameterType.SpecialType == SpecialType.System_Boolean)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Processes a parameter based on its name.
    /// </summary>
    /// <param name="paramName">The parameter name.</param>
    /// <param name="arg">The attribute argument.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="fixedKey">The fixed key value.</param>
    /// <param name="findMethod">The find method name.</param>
    /// <param name="usePrimaryKey">Whether to use the primary key.</param>
    private void ProcessParameter(
        string paramName, 
        AttributeArgumentSyntax arg, 
        Compilation compilation,
        SourceProductionContext context,
        ref string? fixedKey,
        ref string? findMethod,
        ref bool usePrimaryKey)
    {

        if (paramName.Equals("fixedKey", StringComparison.OrdinalIgnoreCase) && 
            arg.Expression is LiteralExpressionSyntax fixedLiteral && 
            fixedLiteral.Kind() == SyntaxKind.StringLiteralExpression)
        {
            fixedKey = fixedLiteral.Token.ValueText;
        }
        else if (paramName.Equals("findMethod", StringComparison.OrdinalIgnoreCase))
        {
            ProcessFindMethodParameter(arg, compilation, context, ref findMethod);
        }
        else if ((paramName.Equals("usePrimaryKey", StringComparison.OrdinalIgnoreCase)) && 
            arg.Expression is LiteralExpressionSyntax usePrimaryKeyLiteral && 
            usePrimaryKeyLiteral.Kind() == SyntaxKind.TrueLiteralExpression)
        {
            usePrimaryKey = true;
        }
    }

    /// <summary>
    /// Processes the findMethod parameter.
    /// </summary>
    /// <param name="arg">The attribute argument.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="findMethod">The find method name.</param>
    private void ProcessFindMethodParameter(
        AttributeArgumentSyntax arg, 
        Compilation compilation,
        SourceProductionContext context,
        ref string? findMethod)
    {
        // Handle both string literals and nameof expressions
        if (arg.Expression is LiteralExpressionSyntax findLiteral && 
            findLiteral.Kind() == SyntaxKind.StringLiteralExpression)
        {
            findMethod = findLiteral.Token.ValueText;
        }
        else if (arg.Expression is InvocationExpressionSyntax invocation)
        {
            ProcessFindMethodInvocation(invocation, arg, compilation, context, ref findMethod);
        }
    }

    /// <summary>
    /// Processes a findMethod parameter that is an invocation expression.
    /// </summary>
    /// <param name="invocation">The invocation expression.</param>
    /// <param name="arg">The attribute argument.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="findMethod">The find method name.</param>
    private void ProcessFindMethodInvocation(
        InvocationExpressionSyntax invocation, 
        AttributeArgumentSyntax arg,
        Compilation compilation,
        SourceProductionContext context,
        ref string? findMethod)
    {
        // Handle nameof() expression
        var invocationSemanticModel = compilation.GetSemanticModel(invocation.SyntaxTree);
        
        // Check if this is a nameof expression
        if (invocation.Expression is IdentifierNameSyntax identifierName && 
            identifierName.Identifier.ValueText == "nameof")
        {
            ProcessNameofExpression(invocation, ref findMethod);
        }
        else
        {
            // Try to get the constant value as a fallback
            var constantValue = invocationSemanticModel.GetConstantValue(invocation);
            
            if (constantValue.HasValue && constantValue.Value is string methodName)
            {
                findMethod = methodName;
            }
        }
    }

    /// <summary>
    /// Processes a nameof expression for a findMethod parameter.
    /// </summary>
    /// <param name="invocation">The invocation expression.</param>
    /// <param name="arg">The attribute argument.</param>
    /// <param name="context">The source production context.</param>
    /// <param name="findMethod">The find method name.</param>
    private void ProcessNameofExpression(
        InvocationExpressionSyntax invocation,
        ref string? findMethod)
    {
        // Extract the argument of the nameof expression
        if (invocation.ArgumentList.Arguments.Count > 0)
        {
            var nameofArg = invocation.ArgumentList.Arguments[0].Expression;
            
            if (nameofArg is IdentifierNameSyntax methodIdentifier)
            {
                findMethod = methodIdentifier.Identifier.ValueText;
            }
        }
    }

    /// <summary>
    /// Validates the find method if one is specified.
    /// </summary>
    /// <param name="findMethod">The find method name.</param>
    /// <param name="interfaceDeclaration">The interface declaration.</param>
    /// <param name="handlerTypeSymbol">The handler type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="context">The source production context.</param>
    /// <returns>True if the find method is valid, false otherwise.</returns>
    private bool ValidateFindMethod(
        string findMethod, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        ITypeSymbol handlerTypeSymbol, 
        Compilation compilation, 
        SourceProductionContext context)
    {
        // Use TypeUtils to find the StatefulQueryHandler or StatefulCommandHandler base class
        // and determine if the handler has an input
        var genericArgs = TypeUtils.GetGenericTypeArguments(handlerTypeSymbol);
        bool hasInput = genericArgs.Count > 2; // Input is present if there are 3 generic arguments
        string? inputType = hasInput ? genericArgs[1].ToString() : null;

        return _keyStrategyValidator.ValidateStaticMethod(
            findMethod, 
            interfaceDeclaration, 
            hasInput,
            inputType, 
            compilation, 
            context);
    }

    /// <summary>
    /// Simplified version of ValidateAttributeArguments that only extracts handler type and alias.
    /// </summary>
    public bool ValidateAttributeArguments(
        AttributeSyntax attribute, 
        InterfaceDeclarationSyntax interfaceDeclaration, 
        Compilation compilation,
        SourceProductionContext context,
        out ITypeSymbol? handlerTypeSymbol,
        out string? alias)
    {
        string? fixedKey;
        string? findMethod;
        bool usePrimaryKey;

        return ValidateAttributeArguments(
            attribute, 
            interfaceDeclaration, 
            compilation, 
            context, 
            out handlerTypeSymbol, 
            out alias, 
            out fixedKey, 
            out findMethod, 
            out usePrimaryKey);
    }

    /// <summary>
    /// Gets the generic type arguments of a type symbol.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>The list of generic type arguments.</returns>
    private System.Collections.Generic.List<ITypeSymbol> GetGenericTypeArguments(ITypeSymbol typeSymbol)
    {
        var namedTypeSymbol = typeSymbol as INamedTypeSymbol;
        if (namedTypeSymbol == null || !namedTypeSymbol.IsGenericType)
        {
            return new System.Collections.Generic.List<ITypeSymbol>();
        }

        return namedTypeSymbol.TypeArguments.ToList();
    }
} 