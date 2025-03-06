using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotr.SourceGen.CqrsCollection.Generators;
using Remotr.SourceGen.CqrsCollection.Validators;

namespace Remotr.SourceGen.CqrsCollection;

/// <summary>
/// Executes the code generation for CqrsCollection attributes.
/// </summary>
public class CqrsCollectionExecutor
{
    private readonly InterfaceValidator _interfaceValidator;
    private readonly AttributeValidator _attributeValidator;
    private readonly HandlerTypeValidator _handlerTypeValidator;
    private readonly StatelessHandlerGenerator _statelessHandlerGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CqrsCollectionExecutor"/> class.
    /// </summary>
    public CqrsCollectionExecutor()
    {
        _interfaceValidator = new InterfaceValidator();
        _attributeValidator = new AttributeValidator();
        _handlerTypeValidator = new HandlerTypeValidator();
        _statelessHandlerGenerator = new StatelessHandlerGenerator();
    }

    /// <summary>
    /// Executes the code generation for an interface declaration with a CqrsCollection attribute.
    /// </summary>
    /// <param name="interfaceDeclaration">The interface declaration to process</param>
    /// <param name="attribute">The CqrsCollection attribute</param>
    /// <param name="compilation">The current compilation</param>
    /// <param name="context">The source production context</param>
    public void Execute(
        InterfaceDeclarationSyntax interfaceDeclaration, 
        AttributeSyntax attribute, 
        Compilation compilation,
        SourceProductionContext context)
    {
        // Check if the interface implements ITransactionManagerGrain
        if (!_interfaceValidator.ImplementsITransactionManagerGrain(interfaceDeclaration, compilation))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR001",
                        "Interface must implement ITransactionManagerGrain",
                        "The interface '{0}' with CqrsCollection attribute must implement ITransactionManagerGrain",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    interfaceDeclaration.Identifier.Text));
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
        if (!_handlerTypeValidator.IsValidHandlerType(handlerTypeSymbol, compilation))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR006",
                        "Handler type must extend StatefulCommandHandler or StatefulQueryHandler",
                        "The handler type '{0}' in CqrsCollection attribute must extend StatefulCommandHandler or StatefulQueryHandler",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    attribute.GetLocation(),
                    handlerTypeSymbol.Name));
            return;
        }

        // Generate the stateless command/query handler
        _statelessHandlerGenerator.Generate(interfaceDeclaration, handlerTypeSymbol, alias, context);
    }
} 