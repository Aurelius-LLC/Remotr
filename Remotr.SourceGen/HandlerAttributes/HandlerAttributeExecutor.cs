using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotr.SourceGen.HandlerAttributes.Generators;
using Remotr.SourceGen.HandlerAttributes.Validators;

namespace Remotr.SourceGen.HandlerAttributes;

/// <summary>
/// Executes the code generation for UseCommand and UseQuery attributes.
/// </summary>
public class HandlerAttributeExecutor
{
    private readonly InterfaceValidator _interfaceValidator;
    private readonly AttributeValidator _attributeValidator;
    private readonly HandlerTypeValidator _handlerTypeValidator;
    private readonly StatelessHandlerGenerator _statelessHandlerGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerAttributeExecutor"/> class.
    /// </summary>
    public HandlerAttributeExecutor()
    {
        _interfaceValidator = new InterfaceValidator();
        _attributeValidator = new AttributeValidator();
        _handlerTypeValidator = new HandlerTypeValidator();
        _statelessHandlerGenerator = new StatelessHandlerGenerator();
    }

    /// <summary>
    /// Executes the code generation for an interface declaration with a UseCommand or UseQuery attribute.
    /// </summary>
    /// <param name="interfaceDeclaration">The interface declaration to process</param>
    /// <param name="attribute">The attribute (UseCommand or UseQuery)</param>
    /// <param name="compilation">The current compilation</param>
    /// <param name="context">The source production context</param>
    public void Execute(
        InterfaceDeclarationSyntax interfaceDeclaration, 
        AttributeSyntax attribute, 
        Compilation compilation,
        SourceProductionContext context)
    {
        // Get attribute name for error messages
        string attributeName = _attributeValidator.GetAttributeTypeName(attribute, compilation);
        
        // Check if the interface implements ITransactionManagerGrain
        if (!_interfaceValidator.ImplementsITransactionManagerGrain(interfaceDeclaration, compilation))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR001",
                        "Interface must implement ITransactionManagerGrain",
                        "The interface '{0}' with {1} attribute must implement ITransactionManagerGrain",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    interfaceDeclaration.Identifier.Text, 
                    attributeName));
            return;
        }

        // Validate attribute arguments
        if (!_attributeValidator.ValidateAttributeArguments(
            attribute, 
            interfaceDeclaration, 
            compilation, 
            context, 
            out var handlerTypeSymbol, 
            out var alias))
        {
            return;
        }

        // Check if the handler type extends StatefulCommandHandler or StatefulQueryHandler
        if (!_handlerTypeValidator.IsValidHandlerType(attributeName == "UseCommandAttribute", handlerTypeSymbol, compilation))
        {
            var queryOrCommand = attributeName == "UseCommandAttribute" ? "StatefulCommandHandler" : "StatefulQueryHandler";
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR006",
                        "Handler type must extend {0}",
                        "The handler type '{1}' in {2} attribute must extend {0}",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    attribute.GetLocation(),
                    queryOrCommand,
                    handlerTypeSymbol.Name,
                    attributeName));
            return;
        }

        // Generate the stateless command/query handler
        _statelessHandlerGenerator.Generate(interfaceDeclaration, handlerTypeSymbol, alias, context);
    }
} 