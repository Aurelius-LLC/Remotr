using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Base class for extension generators that provides common functionality.
/// </summary>
public abstract class BaseExtensionGenerator : IExtensionGenerator
{
    /// <summary>
    /// Gets the base type name that this generator handles.
    /// </summary>
    protected abstract string HandlerBaseTypeName { get; }

    /// <inheritdoc/>
    public bool CanHandle(string baseTypeName)
    {
        return baseTypeName == HandlerBaseTypeName;
    }

    /// <inheritdoc/>
    public abstract void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments);



    /// <summary>
    /// Validates that the number of type arguments is within the expected range.
    /// </summary>
    /// <param name="typeArguments">The type arguments to validate</param>
    /// <param name="minCount">The minimum number of type arguments</param>
    /// <param name="maxCount">The maximum number of type arguments</param>
    /// <returns>True if the type arguments are valid, false otherwise</returns>
    protected bool ValidateTypeArgumentCount(SeparatedSyntaxList<TypeSyntax> typeArguments, int minCount, int maxCount)
    {
        return typeArguments.Count >= minCount && typeArguments.Count <= maxCount;
    }
} 