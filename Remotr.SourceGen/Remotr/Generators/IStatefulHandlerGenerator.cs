using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Interface for stateful handler generators.
/// </summary>
public interface IStatefulHandlerGenerator
{
    /// <summary>
    /// Generates a stateful handler with no input and no output.
    /// </summary>
    /// <param name="sourceBuilder">The source builder</param>
    /// <param name="className">The class name</param>
    /// <param name="stateType">The state type</param>
    void GenerateNoInputNoOutput(
        StringBuilder sourceBuilder, 
        string className, 
        string stateType);

    /// <summary>
    /// Generates a stateful handler with no input and with output.
    /// </summary>
    /// <param name="sourceBuilder">The source builder</param>
    /// <param name="className">The class name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="outputType">The output type</param>
    void GenerateNoInputWithOutput(
        StringBuilder sourceBuilder, 
        string className, 
        string stateType, 
        string outputType);

    /// <summary>
    /// Generates a stateful handler with input and output.
    /// </summary>
    /// <param name="sourceBuilder">The source builder</param>
    /// <param name="className">The class name</param>
    /// <param name="stateType">The state type</param>
    /// <param name="inputType">The input type</param>
    /// <param name="outputType">The output type</param>
    void GenerateWithInputAndOutput(
        StringBuilder sourceBuilder, 
        string className, 
        string stateType, 
        string inputType, 
        string outputType);
} 