using System.Text;

namespace Remotr.SourceGen.CqrsCollection.Generators
{
    /// <summary>
    /// Interface for stateless handler generators with common methods
    /// </summary>
    public interface IStatelessHandlerGenerator
    {

        /// <summary>
        /// Generates a stateless command handler with no input and no output.
        /// </summary>
        void GenerateNoInputNoOutput(
            StringBuilder sb,
            string interfaceName,
            string className,
            string statefulHandlerName,
            string stateType);
            
        /// <summary>
        /// Generates a stateless handler with no input but with output.
        /// </summary>
        void GenerateNoInputWithOutput(
            StringBuilder sb,
            string interfaceName,
            string className,
            string statefulHandlerName,
            string stateType,
            string outputType);

        /// <summary>
        /// Generates a stateless handler with input and output.
        /// </summary>
        void GenerateWithInputAndOutput(
            StringBuilder sb,
            string interfaceName,
            string className,
            string statefulHandlerName,
            string stateType,
            string inputType,
            string outputType);
    }
} 