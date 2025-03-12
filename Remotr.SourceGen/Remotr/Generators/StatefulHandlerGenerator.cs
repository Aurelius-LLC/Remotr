using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Generates stateful handler code.
/// </summary>
public class StatefulHandlerGenerator
{
    private IStatefulHandlerGenerator? _handlerGenerator;
    private readonly IReadOnlyList<IExtensionGenerator> _extensionGenerators;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulHandlerGenerator"/> class.
    /// </summary>
    public StatefulHandlerGenerator()
    {
        _extensionGenerators = new List<IExtensionGenerator>
        {
            new StatefulCommandHandlerGenerator(),
            new StatefulQueryHandlerGenerator()
        };
    }

    /// <summary>
    /// Generates a stateful handler for the given class declaration.
    /// </summary>
    /// <param name="classDeclaration">The class declaration</param>
    /// <param name="context">The source production context</param>
    public void Generate(ClassDeclarationSyntax classDeclaration, SourceProductionContext context)
    {
        var className = classDeclaration.Identifier.Text;
        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
        
        if (baseType is not GenericNameSyntax genericBase)
        {
            return;
        }

        var baseTypeName = genericBase.Identifier.Text;
        var typeArguments = genericBase.TypeArgumentList.Arguments;

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
        handlerGenerator?.GenerateExtensions(sourceBuilder, className, typeArguments);

        sourceBuilder.AppendLine("}");

        context.AddSource($"{extensionClassName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }
} 