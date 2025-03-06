using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Remotr.SourceGen;

/// <summary>
/// Base abstract class providing common functionality for handler generators.
/// </summary>
public abstract class BaseHandlerGenerator : IHandlerGenerator
{
    /// <summary>
    /// The base type name that this generator handles.
    /// </summary>
    protected abstract string HandlerBaseTypeName { get; }

    /// <summary>
    /// Checks if this generator can handle the given base type name.
    /// </summary>
    /// <param name="baseTypeName">Name of the base type to check</param>
    /// <returns>True if this generator can handle the base type, false otherwise</returns>
    public bool CanHandle(string baseTypeName) => baseTypeName == HandlerBaseTypeName;

    /// <summary>
    /// Generates extension methods for the specific handler type.
    /// </summary>
    /// <param name="sb">StringBuilder to append the generated code to</param>
    /// <param name="className">Name of the class being processed</param>
    /// <param name="typeArguments">Type arguments from the base class</param>
    public abstract void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments);

    /// <summary>
    /// Validates that the number of type arguments is within the expected range.
    /// </summary>
    /// <param name="typeArguments">Type arguments to validate</param>
    /// <param name="minArgs">Minimum number of arguments required</param>
    /// <param name="maxArgs">Maximum number of arguments allowed</param>
    /// <returns>True if valid, false otherwise</returns>
    protected bool ValidateTypeArgumentCount(SeparatedSyntaxList<TypeSyntax> typeArguments, int minArgs, int maxArgs)
    {
        return typeArguments.Count >= minArgs && typeArguments.Count <= maxArgs;
    }
} 