using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EG.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ContainerAttribute : Attribute
    {
        public object Key { get; set; }
        public ContainerAttribute() { }
        public ContainerAttribute(object key)
        {
            Key = key;
        }
    }
}
