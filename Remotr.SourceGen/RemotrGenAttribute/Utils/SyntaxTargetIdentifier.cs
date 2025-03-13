using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.RemotrGenAttribute.Utils;

/// <summary>
/// Utility class for identifying syntax targets for code generation.
/// </summary>
public static class SyntaxTargetIdentifier
{
    /// <summary>
    /// Determines if a syntax node should be considered for code generation.
    /// </summary>
    /// <param name="node">The syntax node to check</param>
    /// <returns>True if the node is a class declaration with attributes</returns>
    public static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is ClassDeclarationSyntax { AttributeLists: { Count: > 0 } };
} 