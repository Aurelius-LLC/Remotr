using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.UseShortcutsAttribute.Utils;

/// <summary>
/// Utility class for identifying semantic targets for code generation.
/// </summary>
public static class SemanticTargetIdentifier
{
    /// <summary>
    /// Gets the semantic model target for code generation.
    /// </summary>
    /// <param name="context">The generator syntax context</param>
    /// <returns>The class declaration if it has the UseShortcuts attribute and is valid, null otherwise</returns>
    public static (ClassDeclarationSyntax? ClassDeclaration, bool IsValid) GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        bool hasUseShortcutsAttribute = false;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Remotr.UseShortcutsAttribute")
                {
                    hasUseShortcutsAttribute = true;
                    break;
                }
            }
        }

        if (!hasUseShortcutsAttribute)
        {
            return (null, false);
        }

        // Check if the class extends one of the valid handler types
        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
        if (baseType is not GenericNameSyntax genericBase)
        {
            return (classDeclaration, false);
        }

        var baseTypeName = genericBase.Identifier.Text;
        bool isValidHandler = baseTypeName is "RootQueryHandler" or "RootCommandHandler" 
                            or "EntityQueryHandler" or "EntityCommandHandler";

        return (classDeclaration, isValidHandler);
    }
} 