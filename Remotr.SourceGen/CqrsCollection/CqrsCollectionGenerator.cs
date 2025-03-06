using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotr.SourceGen.CqrsCollection;

[Generator]
public class CqrsCollectionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CqrsCollectionAttribute.g.cs",
            SourceText.From(@"
using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    internal sealed class CqrsCollectionAttribute : Attribute
    {
        public Type HandlerType { get; }
        public string Alias { get; }

        public CqrsCollectionAttribute(Type handlerType, string alias)
        {
            HandlerType = handlerType;
            Alias = alias;
        }
    }
}", Encoding.UTF8)));

        // Get all interface declarations with the CqrsCollection attribute
        IncrementalValueProvider<Compilation> compilationProvider = context.CompilationProvider;

        IncrementalValuesProvider<(InterfaceDeclarationSyntax Interface, AttributeSyntax Attribute, Compilation Compilation)> interfaceDeclarations = 
            context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetSemanticTargetsForGeneration(ctx))
                .Where(static m => m.Interface is not null && m.Attributes.Count > 0)
                .SelectMany((target, _) => 
                    target.Attributes.Select(attr => (target.Interface, Attribute: attr)))
                .Combine(compilationProvider)
                .Select((tuple, _) => (
                    tuple.Left.Interface,
                    tuple.Left.Attribute,
                    tuple.Right));

        // Register the source output
        context.RegisterSourceOutput(interfaceDeclarations,
            (spc, source) => Execute(source.Interface, source.Attribute, source.Compilation, spc));
    }

    /// <summary>
    /// Determines if a syntax node should be considered for code generation.
    /// </summary>
    /// <param name="node">The syntax node to check</param>
    /// <returns>True if the node is an interface declaration with attributes</returns>
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is InterfaceDeclarationSyntax { AttributeLists: { Count: > 0 } };

    /// <summary>
    /// Gets the semantic model targets for code generation.
    /// </summary>
    /// <param name="context">The generator syntax context</param>
    /// <returns>The interface declaration and all CqrsCollection attributes on it</returns>
    private static (InterfaceDeclarationSyntax Interface, List<AttributeSyntax> Attributes) GetSemanticTargetsForGeneration(GeneratorSyntaxContext context)
    {
        var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;
        var cqrsCollectionAttributes = new List<AttributeSyntax>();
        
        foreach (var attributeList in interfaceDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Remotr.CqrsCollectionAttribute")
                {
                    cqrsCollectionAttributes.Add(attribute);
                }
            }
        }

        return (interfaceDeclaration, cqrsCollectionAttributes);
    }

    /// <summary>
    /// Executes the code generation for an interface declaration with a CqrsCollection attribute.
    /// </summary>
    /// <param name="interfaceDeclaration">The interface declaration to process</param>
    /// <param name="attribute">The CqrsCollection attribute</param>
    /// <param name="compilation">The current compilation</param>
    /// <param name="context">The source production context</param>
    private void Execute(
        InterfaceDeclarationSyntax interfaceDeclaration, 
        AttributeSyntax attribute, 
        Compilation compilation,
        SourceProductionContext context)
    {
        // Check if the interface implements ITransactionManagerGrain
        if (!ImplementsITransactionManagerGrain(interfaceDeclaration, compilation))
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

        // Extract attribute arguments
        if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count != 2)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR002",
                        "CqrsCollection attribute must have two arguments",
                        "The CqrsCollection attribute on interface '{0}' must have two arguments: Type handlerType and string alias",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    attribute.GetLocation(),
                    interfaceDeclaration.Identifier.Text));
            return;
        }

        // Get the handler type from the attribute
        var handlerTypeArgument = attribute.ArgumentList.Arguments[0];
        var aliasArgument = attribute.ArgumentList.Arguments[1];

        if (handlerTypeArgument.Expression is not TypeOfExpressionSyntax typeOfExpression)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR003",
                        "First argument must be a type",
                        "The first argument of CqrsCollection attribute on interface '{0}' must be a type expression (typeof(...))",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    handlerTypeArgument.GetLocation(),
                    interfaceDeclaration.Identifier.Text));
            return;
        }

        if (aliasArgument.Expression is not LiteralExpressionSyntax aliasLiteral || 
            aliasLiteral.Kind() != SyntaxKind.StringLiteralExpression)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR004",
                        "Second argument must be a string",
                        "The second argument of CqrsCollection attribute on interface '{0}' must be a string literal",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    aliasArgument.GetLocation(),
                    interfaceDeclaration.Identifier.Text));
            return;
        }

        var handlerTypeInfo = compilation.GetSemanticModel(typeOfExpression.SyntaxTree)
            .GetTypeInfo(typeOfExpression.Type);

        if (handlerTypeInfo.Type == null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR005",
                        "Handler type not found",
                        "The handler type in CqrsCollection attribute on interface '{0}' could not be resolved",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    typeOfExpression.GetLocation(),
                    interfaceDeclaration.Identifier.Text));
            return;
        }

        var handlerTypeSymbol = handlerTypeInfo.Type;
        var alias = aliasLiteral.Token.ValueText;

        // Check if the handler type extends StatefulCommandHandler or StatefulQueryHandler
        if (!IsValidHandlerType(handlerTypeSymbol, compilation))
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
                    typeOfExpression.GetLocation(),
                    handlerTypeSymbol.Name));
            return;
        }

        // Ensure the alias is different from the handler type name
        if (handlerTypeSymbol.Name == alias)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "REMOTR007",
                        "Alias must be different from handler type name",
                        "The alias '{0}' in CqrsCollection attribute must be different from the handler type name",
                        "Remotr",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    aliasArgument.GetLocation(),
                    alias));
            return;
        }

        // Generate the stateless command/query handler
        GenerateStatelessHandler(interfaceDeclaration, handlerTypeSymbol, alias, compilation, context);
    }

    private bool ImplementsITransactionManagerGrain(InterfaceDeclarationSyntax interfaceDeclaration, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);
        var interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration) as INamedTypeSymbol;
        
        if (interfaceSymbol == null)
            return false;

        var iTransactionManagerGrainSymbol = compilation.GetTypeByMetadataName("Remotr.ITransactionManagerGrain");
        
        if (iTransactionManagerGrainSymbol == null)
            return false;

        return interfaceSymbol.AllInterfaces.Contains(iTransactionManagerGrainSymbol, SymbolEqualityComparer.Default);
    }

    private bool IsValidHandlerType(ITypeSymbol handlerTypeSymbol, Compilation compilation)
    {
        // Try different namespace possibilities
        string[] possibleNamespaces = new[] { "Remotr" };
        INamedTypeSymbol statefulCommandHandlerSymbol = null;
        INamedTypeSymbol statefulQueryHandlerSymbol = null;

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
                compilation.GetTypeByMetadataName($"{prefix}StatefulQueryHandler`2");

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
            if (currentName.Contains("StatefulCommandHandler") || currentName.Contains("StatefulQueryHandler"))
                return true;
                
            current = current.BaseType;
        }
        
        // Fallback to original check if direct name check didn't succeed
        if (statefulCommandHandlerSymbol == null && statefulQueryHandlerSymbol == null)
            return false;

        bool isCommandHandler = statefulCommandHandlerSymbol != null && 
                              IsSubtypeOfOpenGeneric(handlerTypeSymbol, statefulCommandHandlerSymbol);
                              
        bool isQueryHandler = statefulQueryHandlerSymbol != null && 
                            IsSubtypeOfOpenGeneric(handlerTypeSymbol, statefulQueryHandlerSymbol);
                            
        return isCommandHandler || isQueryHandler;
    }

    private bool IsSubtypeOfOpenGeneric(ITypeSymbol type, INamedTypeSymbol openGenericType)
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

    private void GenerateStatelessHandler(
        InterfaceDeclarationSyntax interfaceDeclaration, 
        ITypeSymbol handlerTypeSymbol, 
        string alias,
        Compilation compilation,
        SourceProductionContext context)
    {
        var handlerName = handlerTypeSymbol.Name;
        var isCommandHandler = IsCommandHandler(handlerTypeSymbol);
        var genericTypeArgs = GetGenericTypeArguments(handlerTypeSymbol);
        
        if (genericTypeArgs.Count == 0)
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
        var namespaceName = GetNamespace(interfaceDeclaration);
        var interfaceName = interfaceDeclaration.Identifier.Text;

        // Build stateless handler
        var sourceBuilder = new StringBuilder();
        sourceBuilder.AppendLine("// This file was generated by the Remotr.SourceGen CqrsCollectionGenerator.");
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

        // First generic parameter is the state type, we need to extract the others
        var stateType = genericTypeArgs[0].ToString();
        
        // Create the appropriate stateless handler class
        if (isCommandHandler)
        {
            switch (genericTypeArgs.Count)
            {
                case 1: // StatefulCommandHandler<TState>
                    GenerateStatelessCommandHandlerNoInputNoOutput(sourceBuilder, interfaceName, alias, handlerName, stateType);
                    break;
                case 2: // StatefulCommandHandler<TState, TOutput>
                    GenerateStatelessCommandHandlerNoInputWithOutput(sourceBuilder, interfaceName, alias, handlerName, stateType, genericTypeArgs[1].ToString());
                    break;
                case 3: // StatefulCommandHandler<TState, TInput, TOutput>
                    GenerateStatelessCommandHandlerWithInputAndOutput(sourceBuilder, interfaceName, alias, handlerName, stateType, genericTypeArgs[1].ToString(), genericTypeArgs[2].ToString());
                    break;
            }
        }
        else // QueryHandler
        {
            switch (genericTypeArgs.Count)
            {
                case 1: // StatefulQueryHandler<TState>
                    GenerateStatelessQueryHandlerNoOutput(sourceBuilder, interfaceName, alias, handlerName, stateType);
                    break;
                case 2: // StatefulQueryHandler<TState, TOutput>
                    GenerateStatelessQueryHandlerWithOutput(sourceBuilder, interfaceName, alias, handlerName, stateType, genericTypeArgs[1].ToString());
                    break;
            }
        }

        context.AddSource($"{alias}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    private bool IsCommandHandler(ITypeSymbol handlerTypeSymbol)
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

    private List<ITypeSymbol> GetGenericTypeArguments(ITypeSymbol handlerTypeSymbol)
    {
        // Find the StatefulCommandHandler or StatefulQueryHandler base class and get its type arguments
        var current = handlerTypeSymbol;
        
        while (current != null)
        {
            if (current is INamedTypeSymbol namedType && 
                (current.Name.Contains("StatefulCommandHandler") || current.Name.Contains("StatefulQueryHandler")))
            {
                return namedType.TypeArguments.ToList();
            }
            
            current = current.BaseType;
        }
        
        return new List<ITypeSymbol>();
    }

    private string GetNamespace(InterfaceDeclarationSyntax interfaceDeclaration)
    {
        // Get the namespace from the syntax tree
        var namespaceDecl = interfaceDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        return namespaceDecl?.Name.ToString() ?? string.Empty;
    }

    private void GenerateStatelessCommandHandlerNoInputNoOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType)
    {
        sb.AppendLine($"public class {className} : StatelessCommandHandler<{interfaceName}>");
        sb.AppendLine("{");
        sb.AppendLine("    public override async Task Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        await CommandFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Tell<{statefulHandlerName}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }

    private void GenerateStatelessCommandHandlerNoInputWithOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string outputType)
    {
        sb.AppendLine($"public class {className} : StatelessCommandHandler<{interfaceName}, {outputType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public override async Task<{outputType}> Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await CommandFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Tell<{statefulHandlerName}, {outputType}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }

    private void GenerateStatelessCommandHandlerWithInputAndOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string inputType,
        string outputType)
    {
        sb.AppendLine($"public class {className} : StatelessCommandHandler<{interfaceName}, {inputType}, {outputType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public override async Task<{outputType}> Execute({inputType} input)");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await CommandFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Tell<{statefulHandlerName}, {inputType}, {outputType}>(input)");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }

    private void GenerateStatelessQueryHandlerNoOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType)
    {
        sb.AppendLine($"public class {className} : StatelessQueryHandler<{interfaceName}>");
        sb.AppendLine("{");
        sb.AppendLine("    public override async Task Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        await QueryFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Ask<{statefulHandlerName}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }

    private void GenerateStatelessQueryHandlerWithOutput(
        StringBuilder sb, 
        string interfaceName, 
        string className, 
        string statefulHandlerName,
        string stateType,
        string outputType)
    {
        sb.AppendLine($"public class {className} : StatelessQueryHandler<{interfaceName}, {outputType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public override async Task<{outputType}> Execute()");
        sb.AppendLine("    {");
        sb.AppendLine($"        return await QueryFactory.GetChild<{stateType}>()");
        sb.AppendLine($"            .Ask<{statefulHandlerName}, {outputType}>()");
        sb.AppendLine("            .Run(GetPrimaryKeyString());");
        sb.AppendLine("    }");
        sb.AppendLine("}");
    }
} 