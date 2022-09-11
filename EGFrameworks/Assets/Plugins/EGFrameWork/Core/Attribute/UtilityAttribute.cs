using System;
using EG.Core;

namespace EG
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UtilityAttribute : ContainerAttribute
    {
        public UtilityAttribute() : base() { }
        public UtilityAttribute(object key) : base(key) { }
    }
}
