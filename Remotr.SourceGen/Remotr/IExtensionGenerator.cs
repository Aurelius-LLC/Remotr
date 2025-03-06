using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Interface defining the contract for generating extensions for handlers.
/// </summary>
public interface IExtensionGenerator
{
    /// <summary>
    /// Generates extension methods for the specific handler type.
    /// </summary>
    /// <param name="sb">StringBuilder to append the generated code to</param>
    /// <param name="className">Name of the class being processed</param>
    /// <param name="typeArguments">Type arguments from the base class</param>
    void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments);

    /// <summary>
    /// Checks if this generator can handle the given base type name.
    /// </summary>
    /// <param name="baseTypeName">Name of the base type to check</param>
    /// <returns>True if this generator can handle the base type, false otherwise</returns>
    bool CanHandle(string baseTypeName);
} 