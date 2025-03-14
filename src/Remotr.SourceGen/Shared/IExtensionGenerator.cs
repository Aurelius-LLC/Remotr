using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Remotr.SourceGen.Shared;

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
    /// <param name="implementationGenericTypes">Generic type parameters of the implementation class (e.g. "<T1, T2>")</param>
    /// <param name="genericConstraints">Generic constraints of the implementation class (e.g. "where T1 : ITest")</param>
    /// <param name="includesImplementationGenerics">Whether the implementation class has its own generic type parameters</param>
    void GenerateExtensions(
        StringBuilder sb, 
        string className, 
        SeparatedSyntaxList<TypeSyntax> typeArguments,
        string implementationGenericTypes = "",
        string implementationGenericTypesWithOther = "<T>",
        string genericConstraints = "",
        string otherGenericType = "T");

    /// <summary>
    /// Checks if this generator can handle the given base type name.
    /// </summary>
    /// <param name="baseTypeName">Name of the base type to check</param>
    /// <returns>True if this generator can handle the base type, false otherwise</returns>
    bool CanHandle(string baseTypeName);
} 