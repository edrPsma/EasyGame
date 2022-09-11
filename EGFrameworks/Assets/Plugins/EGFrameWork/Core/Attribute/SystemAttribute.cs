using System;
using EG.Core;

namespace EG
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SystemAttribute : ContainerAttribute
    {
        public SystemAttribute() : base() { }
        public SystemAttribute(object key) : base(key) { }
    }
}
