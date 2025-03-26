using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Remotr.SourceGen.UseShortcutsAttribute;

/// <summary>
/// Generates the UseShortcuts attribute source code.
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
            ctx.AddSource("UseShortcutsAttribute.g.cs", SourceText.From(GetUseShortcutsAttributeSource(), Encoding.UTF8));
        });
    }

    /// <summary>
    /// Gets the source code for the UseShortcutsAttribute.
    /// </summary>
    /// <returns>The attribute source code.</returns>
    private string GetUseShortcutsAttributeSource()
    {
        return @"
using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class UseShortcutsAttribute : Attribute
    {
        public UseShortcutsAttribute() { }
    }
}";
    }
} 