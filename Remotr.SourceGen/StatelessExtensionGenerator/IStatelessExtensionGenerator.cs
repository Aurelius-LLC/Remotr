using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.StatelessExtensionGenerators;

/// <summary>
/// Interface for stateless extension generators.
/// </summary>
public interface IStatelessExtensionGenerator : IExtensionGenerator
{
    /// <summary>
    /// Generates extensions with no input and no output.
    /// </summary>
    void GenerateNoInputNoOutput(StringBuilder sb, string className, string stateType);

    /// <summary>
    /// Generates extensions with no input but with output.
    /// </summary>
    void GenerateNoInputWithOutput(StringBuilder sb, string className, string stateType, string outputType);

    /// <summary>
    /// Generates extensions with input and output.
    /// </summary>
    void GenerateWithInputAndOutput(StringBuilder sb, string className, string stateType, string inputType, string outputType);
} 