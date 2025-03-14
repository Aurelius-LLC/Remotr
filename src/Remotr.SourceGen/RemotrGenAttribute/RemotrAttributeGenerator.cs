using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Remotr.SourceGen.RemotrGenAttribute;

/// <summary>
/// Generates the RemotrGen attribute source code.
/// </summary>
public class RemotrAttributeGenerator
{
    /// <summary>
    /// Registers the attribute source with the generator context.
    /// </summary>
    /// <param name="context">The generator initialization context.</param>
    public void RegisterAttributeSource(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => 
        {
            ctx.AddSource("RemotrGenAttribute.g.cs", SourceText.From(GetRemotrGenAttributeSource(), Encoding.UTF8));
        });
    }

    /// <summary>
    /// Gets the source code for the RemotrGenAttribute.
    /// </summary>
    /// <returns>The attribute source code.</returns>
    private string GetRemotrGenAttributeSource()
    {
        return @"
using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RemotrGenAttribute : Attribute
    {
        public RemotrGenAttribute() { }
    }
}";
    }
} 