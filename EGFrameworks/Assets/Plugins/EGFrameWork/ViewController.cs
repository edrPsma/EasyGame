using System;
using System.Collections;
using System.Collections.Generic;
using EG.Core;
using UnityEngine;

namespace EG
{
    public class ViewController : MonoBehaviour, IView
    {
        public void SendCommand<T>() where T : ICommand, new()
        {
            new T().Execute();
        }

        public void SendCommand<T>(T instance) where T : ICommand, new()
        {
            instance.Execute();
        }

        public IUnRegister EventRegister<T>(Action<T> onEvent, bool UnRegisterWhenGameObjectDestory = true) where T : new()
        {
            var Event = EasyEvent.Instance;
            if (UnRegisterWhenGameObjectDestory)
            {
                Event.Register<T>(onEvent).UnRegisterWhenGameObjectDestory(this.gameObject);
                return null;
            }
            else
            {
                return EasyEvent.Instance.Register<T>(onEvent);
            }
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
