using System;
using EG.Core;
using UnityEngine;

namespace EG
{
    public class AbstractSystem : MonoBehaviour, ISystem
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

        public IUnRegister EventRegister<T>(Action<T> onEvent) where T : new()
        {
            return EasyEvent.Instance.Register<T>(onEvent);
        }

        public void EventUnRegister<T>(Action<T> onEvent)
        {
            EasyEvent.Instance.UnRegister<T>(onEvent);
        }

        public T GetSystem<T>() where T : class, ISystem
        {
            return ContainerManager.Instance.GetSystem<T>() as T;
        }

        public object GetSystem(object key)
        {
            return ContainerManager.Instance.GetSystem(key);
        }

        public T GetModel<T>() where T : class, IModel
        {
            return ContainerManager.Instance.GetModel<T>() as T;
        }

        public object GetModel(object key)
        {
            return ContainerManager.Instance.GetModel(key);
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
