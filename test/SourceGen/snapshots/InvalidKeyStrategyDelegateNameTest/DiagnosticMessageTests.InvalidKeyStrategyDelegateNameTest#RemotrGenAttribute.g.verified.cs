//HintName: RemotrGenAttribute.g.cs

using System;
namespace Remotr
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class RemotrGenAttribute : Attribute
    {
        public RemotrGenAttribute() { }
    }
}