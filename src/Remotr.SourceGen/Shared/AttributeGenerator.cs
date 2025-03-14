using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Remotr.SourceGen.Shared;

/// <summary>
/// Generates the UseCommand and UseQuery attribute source code.
/// </summary>
public class AttributeGenerator
{
    /// <summary>
    /// Registers the attribute sources with the generator context.
    /// </summary>
    /// <param name="context">The generator initialization context.</param>
    public void RegisterAttributeSource(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => 
        {
            ctx.AddSource("UseCommandAttribute.g.cs", SourceText.From(GetUseCommandAttributeSource(), Encoding.UTF8));
            ctx.AddSource("UseQueryAttribute.g.cs", SourceText.From(GetUseQueryAttributeSource(), Encoding.UTF8));
        });
    }

    /// <summary>
    /// Gets the source code for the UseCommandAttribute.
    /// </summary>
    /// <returns>The attribute source code.</returns>
    private string GetUseCommandAttributeSource()
    {
        return @"
using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    internal sealed class UseCommandAttribute : Attribute
    {
        public Type HandlerType { get; }
        public string Alias { get; }
        public string FixedKey { get; }
        public string FindMethod { get; }
        public bool UsePrimaryKey { get; }

        public UseCommandAttribute(Type handlerType, string alias)
        {
            HandlerType = handlerType;
            Alias = alias;
            FixedKey = null;
            FindMethod = null;
            UsePrimaryKey = true;
        }

        public UseCommandAttribute(Type handlerType, string alias, string fixedKey = null, string findMethod = null, bool usePrimaryKey = false)
        {
            HandlerType = handlerType;
            Alias = alias;
            FixedKey = fixedKey;
            FindMethod = findMethod;
            UsePrimaryKey = fixedKey == null && findMethod == null && !usePrimaryKey ? true : usePrimaryKey;
        }
    }
}";
    }

    /// <summary>
    /// Gets the source code for the UseQueryAttribute.
    /// </summary>
    /// <returns>The attribute source code.</returns>
    private string GetUseQueryAttributeSource()
    {
        return @"
using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    internal sealed class UseQueryAttribute : Attribute
    {
        public Type HandlerType { get; }
        public string Alias { get; }
        public string FixedKey { get; }
        public string FindMethod { get; }
        public bool UsePrimaryKey { get; }

        public UseQueryAttribute(Type handlerType, string alias)
        {
            HandlerType = handlerType;
            Alias = alias;
            FixedKey = null;
            FindMethod = null;
            UsePrimaryKey = true;
        }

        public UseQueryAttribute(Type handlerType, string alias, string fixedKey = null, string findMethod = null, bool usePrimaryKey = false)
        {
            HandlerType = handlerType;
            Alias = alias;
            FixedKey = fixedKey;
            FindMethod = findMethod;
            UsePrimaryKey = fixedKey == null && findMethod == null && !usePrimaryKey ? true : usePrimaryKey;
        }
    }
}";
    }
}