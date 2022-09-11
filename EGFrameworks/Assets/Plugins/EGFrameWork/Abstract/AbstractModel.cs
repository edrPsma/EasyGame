using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Core;

namespace EG
{
    public abstract class AbstractModel : MonoBehaviour, IModel
    {
        void IContainable.Init()
        {
            OnInit();
        }

        protected virtual void OnInit() { }

        public void EventTrigger<T>() where T : new()
        {
            EasyEvent.Instance.Trigger<T>();
        }

        public void EventTrigger<T>(T instance) where T : new()
        {
            EasyEvent.Instance.Trigger<T>(instance);
        }

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
