using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Core;

namespace EG
{
    public class AbstractUtility : MonoBehaviour, IUtility
    {
        void IContainable.Init()
        {
            OnInit();
        }

        protected virtual void OnInit() { }

        public T GetUtility<T>() where T : class, IUtility
        {
            return ContainerManager.Instance.GetUtility<T>() as T;
        }

        public object GetUtility(object key)
        {
            return ContainerManager.Instance.GetUtility(key);
        }
    }
}
