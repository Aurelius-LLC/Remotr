using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;
using Microsoft.CodeAnalysis.Text;

namespace Remotr.SourceGen.UseHandlerAttributes.ExtensionGenerators;

/// <summary>
/// Generates extensions for handlers.
/// </summary>
public class ExtensionsGeneratorExecutor(List<IExtensionGenerator> extensionGenerators)
{
    private readonly List<IExtensionGenerator> _extensionGenerators = extensionGenerators;


    /// <summary>
    /// Generates the stateless extensions for the given class declaration.
    /// </summary>
    /// <param name="classDeclaration">The class declaration</param>
    /// <param name="context">The source production context</param>
    public void Generate(
        ClassDeclarationSyntax classDeclaration,
        SourceProductionContext context
    ) {
        
        var className = classDeclaration.Identifier.Text;
        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
        
        if (baseType is not GenericNameSyntax genericBase)
        {
            return;
        }

        var baseTypeName = genericBase.Identifier.Text;
        var typeArguments = genericBase.TypeArgumentList.Arguments;

        // Extract implementation generic types if present
        var implementationGenericTypes = "";
        var genericConstraints = "";
        var implementationGenericTypesWithOther = "<T>";
        var otherGenericType = "T";

        if (classDeclaration.TypeParameterList != null)
        {
            implementationGenericTypes = classDeclaration.TypeParameterList.ToString();
            implementationGenericTypesWithOther = implementationGenericTypes.Substring(0, implementationGenericTypes.Length - 1) + ", TOther>";
            otherGenericType = "TOther";
            
            // Extract constraints if present
            if (classDeclaration.ConstraintClauses.Any())
            {
                genericConstraints = " " + string.Join(" ", classDeclaration.ConstraintClauses.Select(c => c.ToString()));
            }
        }

        // Get the namespace from the source file
        var namespaceDecl = classDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        var namespaceName = namespaceDecl?.Name.ToString()!;

        var sourceBuilder = new StringBuilder();
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using System.Threading.Tasks;");
        sourceBuilder.AppendLine("using Remotr;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine($"namespace {namespaceName};");
        sourceBuilder.AppendLine();

        var t1 = typeArguments[0].ToString();
        var extensionClassName = $"{t1}{className}";

        sourceBuilder.AppendLine($"public static class {extensionClassName}");
        sourceBuilder.AppendLine("{");

        // Initialize the appropriate handler generator based on the base type name
        var handlerGenerator = _extensionGenerators.FirstOrDefault(g => g.CanHandle(baseTypeName));
        handlerGenerator?.GenerateExtensions(
            sourceBuilder, 
            className, 
            typeArguments,
            implementationGenericTypes,
            implementationGenericTypesWithOther,
            genericConstraints,
            otherGenericType);

        sourceBuilder.AppendLine("}");

        context.AddSource($"{extensionClassName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    /// <summary>
    /// Generates handler extensions based on the number of generic type arguments.
    /// </summary>
    /// <param name="sourceBuilder">The source builder</param>
    /// <param name="interfaceName">The interface name</param>
    /// <param name="alias">The alias</param>
    /// <param name="handlerName">The handler name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="baseGenericTypeArgs">The base generic type arguments</param>
    /// <param name="fixedKey">The fixed key, if specified</param>
    /// <param name="findMethod">The find method name, if specified</param>
    /// <param name="usePrimaryKey">Whether to use the primary key</param>
    /// <param name="namespaceName">The namespace name</param>
    /// <param name="isCommandHandler">Whether this is a command handler</param>
    /// <param name="interfaceType">The interface type</param>
    public void GenerateExtensions(
        StringBuilder sourceBuilder, 
        string interfaceName, 
        string alias,
        List<ITypeSymbol> baseGenericTypeArgs,
        bool isCommandHandler,
        string interfaceType)
    {
        // Generate extensions using RemotrGen generators
        var extensionsBuilder = new StringBuilder();
        extensionsBuilder.AppendLine();
        extensionsBuilder.AppendLine($"public static class {interfaceName}{alias}Extensions");
        extensionsBuilder.AppendLine("{");

        var baseTypeName = isCommandHandler ? "RootCommandHandler" : "RootQueryHandler";
        var generator = _extensionGenerators.FirstOrDefault(g => g.CanHandle(baseTypeName));
        if (generator != null)
        {
            var typeArgList = new SeparatedSyntaxList<TypeSyntax>();
            // Replace first generic argument (state type) with interface type
            typeArgList = typeArgList.Add(SyntaxFactory.ParseTypeName(interfaceType));
            // Add remaining type arguments
            for (int i = 1; i < baseGenericTypeArgs.Count; i++)
            {
                typeArgList = typeArgList.Add(SyntaxFactory.ParseTypeName(baseGenericTypeArgs[i].ToString()));
            }
            generator.GenerateExtensions(extensionsBuilder, alias, typeArgList);
        }

        extensionsBuilder.AppendLine("}");
        sourceBuilder.Append(extensionsBuilder);
    }
}
