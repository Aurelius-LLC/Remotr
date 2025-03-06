using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Remotr.SourceGen.CqrsCollection;

/// <summary>
/// Generates the CqrsCollectionAttribute source code.
/// </summary>
public class AttributeGenerator
{
    /// <summary>
    /// Registers the attribute source with the generator context.
    /// </summary>
    /// <param name="context">The generator initialization context.</param>
    public void RegisterAttributeSource(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "CqrsCollectionAttribute.g.cs",
            SourceText.From(GetAttributeSource(), Encoding.UTF8)));
    }

    /// <summary>
    /// Gets the source code for the CqrsCollectionAttribute.
    /// </summary>
    /// <returns>The attribute source code.</returns>
    private string GetAttributeSource()
    {
        return @"
using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    internal sealed class CqrsCollectionAttribute : Attribute
    {
        public Type HandlerType { get; }
        public string Alias { get; }

        public CqrsCollectionAttribute(Type handlerType, string alias)
        {
            HandlerType = handlerType;
            Alias = alias;
        }
    }
}";
    }
} 