//HintName: UseShortcutsAttribute.g.cs

using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class UseShortcutsAttribute : Attribute
    {
        public UseShortcutsAttribute() { }
    }
}