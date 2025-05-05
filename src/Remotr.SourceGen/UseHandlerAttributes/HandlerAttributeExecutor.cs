using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Remotr.SourceGen.UseHandlerAttributes.ExtensionGenerators;
using Remotr.SourceGen.UseHandlerAttributes.HandlerGenerators;
using Remotr.SourceGen.UseHandlerAttributes.Utils;
using Remotr.SourceGen.UseHandlerAttributes.Validators;
using Remotr.StatelessExtensionGenerators;

namespace Remotr.SourceGen.UseHandlerAttributes;

/// <summary>
/// Executes the code generation for UseCommand and UseQuery attributes.
/// </summary>
public class HandlerAttributeExecutor
{
    private readonly InterfaceValidator _interfaceValidator;
    private readonly AttributeValidator _attributeValidator;
    private readonly HandlerTypeValidator _handlerTypeValidator;
    private readonly HandlersGenerator _handlersGenerator;
    private readonly ExtensionsGeneratorExecutor _extensionsGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerAttributeExecutor"/> class.
    /// </summary>
    public HandlerAttributeExecutor()
    {
        _interfaceValidator = new InterfaceValidator();
        _attributeValidator = new AttributeValidator();
        _handlerTypeValidator = new HandlerTypeValidator();
        _handlersGenerator = new HandlersGenerator();
        _extensionsGenerator = new ExtensionsGeneratorExecutor(
        [
            new RootCommandExtensionGenerator(),
            new RootQueryExtensionGenerator()
        ]);
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
        
        // Check if the interface implements IAggregateRoot
        if (!_interfaceValidator.ImplementsIAggregateRoot(interfaceDeclaration, compilation))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR001",
                        "Interface must implement IAggregateRoot",
                        "The interface '{0}' with {1} attribute must implement IAggregateRoot",
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
            out var alias,
            out var fixedKey,
            out var findMethod,
            out var usePrimaryKey))
        {
            return;
        }

        // Check if the handler type extends EntityCommandHandler or EntityQueryHandler
        if (!_handlerTypeValidator.IsValidHandlerType(attributeName == "UseCommandAttribute", handlerTypeSymbol!, compilation))
        {
            var queryOrCommand = attributeName == "UseCommandAttribute" ? "EntityCommandHandler" : "EntityQueryHandler";
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


        var handlerName = handlerTypeSymbol.Name;
        var isCommandHandler = _handlerTypeValidator.IsCommandHandler(handlerTypeSymbol);
        
        var genericTypeArgs = TypeUtils.GetGenericTypeArguments(handlerTypeSymbol);
        var baseGenericTypeArgs = TypeUtils.GetBaseGenericTypeArguments(handlerTypeSymbol);
        
        if (baseGenericTypeArgs.Count == 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR008",
                        "Could not determine generic type arguments",
                        "Could not determine generic type arguments for handler type '{0}'",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    interfaceDeclaration.GetLocation(),
                    handlerTypeSymbol.Name));
            return;
        }

        // Get namespace and interface name
        var namespaceName = TypeUtils.GetNamespace(interfaceDeclaration);
        var interfaceName = interfaceDeclaration.Identifier.Text;

        // Build stateless handler
        var sourceBuilder = new StringBuilder();
        sourceBuilder.AppendLine("// This file was generated by the Remotr.SourceGen HandlerAttributeGenerator.");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using System.Threading.Tasks;");
        sourceBuilder.AppendLine("using Remotr;");
        sourceBuilder.AppendLine();
        
        if (!string.IsNullOrEmpty(namespaceName))
        {
            sourceBuilder.AppendLine($"namespace {namespaceName};");
            sourceBuilder.AppendLine();
        }

        // First generic parameter is the state type
        var stateType = baseGenericTypeArgs[0].ToString();
        var interfaceType = $"{namespaceName}.{interfaceName}";

        // Generate the stateless command/query handler
        _handlersGenerator.GenerateHandlers(sourceBuilder, namespaceName, handlerName, stateType, interfaceName, genericTypeArgs, baseGenericTypeArgs, isCommandHandler, alias!, fixedKey, findMethod);

        // Generate the extensions
        _extensionsGenerator.GenerateExtensions(sourceBuilder, interfaceName, alias!, baseGenericTypeArgs, isCommandHandler, interfaceType);

        context.AddSource($"{alias}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }
} 