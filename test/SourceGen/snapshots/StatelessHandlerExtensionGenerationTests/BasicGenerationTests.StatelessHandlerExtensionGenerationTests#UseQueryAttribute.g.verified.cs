//HintName: UseQueryAttribute.g.cs

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
}