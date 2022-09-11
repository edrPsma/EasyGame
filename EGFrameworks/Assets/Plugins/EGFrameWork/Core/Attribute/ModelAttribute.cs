using System;
using EG.Core;

namespace EG
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModelAttribute : ContainerAttribute
    {
        public ModelAttribute() : base() { }
        public ModelAttribute(object key) : base(key) { }
    }
}
