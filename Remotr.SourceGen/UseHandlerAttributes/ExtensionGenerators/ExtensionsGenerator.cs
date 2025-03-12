using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.SourceGen.UseHandlerAttributes.ExtensionGenerators;

/// <summary>
/// Generates extensions for handlers.
/// </summary>
public class ExtensionsGenerator
{
    private readonly IReadOnlyList<IExtensionGenerator> _extensionGenerators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensionsGenerator"/> class.
    /// </summary>
    public ExtensionsGenerator()
    {
        _extensionGenerators = new List<IExtensionGenerator>
        {
            new StatelessCommandExtensionGenerator(),
            new StatelessQueryExtensionGenerator()
        };
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

        var baseTypeName = isCommandHandler ? "StatelessCommandHandler" : "StatelessQueryHandler";
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
