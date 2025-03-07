using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.HandlerAttributes.Utils;

/// <summary>
/// Identifies syntax nodes that are potential targets for code generation.
/// </summary>
public static class SyntaxTargetIdentifier
{
    /// <summary>
    /// Determines if a syntax node should be considered for code generation.
    /// </summary>
    /// <param name="node">The syntax node to check</param>
    /// <returns>True if the node is an interface declaration with attributes</returns>
    public static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is InterfaceDeclarationSyntax { AttributeLists: { Count: > 0 } };
} 