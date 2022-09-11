using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.UI;
using EG.Scene.Core;
using EG.UI.Core;
using EG.Core;
using System;

namespace EG.Scene
{
    public abstract class BaseSceneState : IBaseSceneState
    {
        public string ScenePath { get; set; }
        private List<object> mContainableList = new List<object>();

        public void LoadStart()
        {
            OnLoadStart();
        }

        void IBaseSceneState.EnterScene()
        {
            OnEnterScene();
        }

        void IBaseSceneState.ExitScene()
        {
            OnExitScene();
            UnRegisterWhenSceneExit();
        }

        public BaseSceneState()
        {
            ScenePath = BindScene();
        }
        protected abstract string BindScene();
        protected virtual void OnEnterScene() { }
        protected virtual void OnExitScene() { }
        protected virtual void OnLoadStart() { }


        #region 职能实现
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

        public void RegisterSystem<T>(object key = null, bool unRegisterWhenSceneExit = true) where T : class, ISystem
        {
            if (unRegisterWhenSceneExit)
            {
                if (key == null)
                {
                    mContainableList.Add(typeof(T));
                }
                else
                {
                    mContainableList.Add(key);
                }
            }
            ContainerManager.Instance.RegisterSystem<T>(key);
        }

        public void RegisterSystem(Type systemType, object key = null, bool unRegisterWhenSceneExit = true)
        {
            if (unRegisterWhenSceneExit)
            {
                if (key == null)
                {
                    mContainableList.Add(systemType);
                }
                else
                {
                    mContainableList.Add(key);
                }
            }
            ContainerManager.Instance.RegisterSystem(systemType, key);
        }

        public void RegisterModel<T>(object key = null, bool unRegisterWhenSceneExit = true) where T : class, IModel
        {
            if (unRegisterWhenSceneExit)
            {
                if (key == null)
                {
                    mContainableList.Add(typeof(T));
                }
                else
                {
                    mContainableList.Add(key);
                }
            }
            ContainerManager.Instance.RegisterModel<T>(key);
        }

        public void RegisterModel(Type modelType, object key = null, bool unRegisterWhenSceneExit = true)
        {
            if (unRegisterWhenSceneExit)
            {
                if (key == null)
                {
                    mContainableList.Add(modelType);
                }
                else
                {
                    mContainableList.Add(key);
                }
            }
            ContainerManager.Instance.RegisterModel(modelType, key);
        }

        public void RegisterUtility<T>(object key = null, bool unRegisterWhenSceneExit = true) where T : class, IUtility
        {
            if (unRegisterWhenSceneExit)
            {
                if (key == null)
                {
                    mContainableList.Add(typeof(T));
                }
                else
                {
                    mContainableList.Add(key);
                }
            }
            ContainerManager.Instance.RegisterUtility<T>(key);
        }

        public void RegisterUtility(Type utilityType, object key = null, bool unRegisterWhenSceneExit = true)
        {
            if (unRegisterWhenSceneExit)
            {
                if (key == null)
                {
                    mContainableList.Add(utilityType);
                }
                else
                {
                    mContainableList.Add(key);
                }
            }
            ContainerManager.Instance.RegisterUtility(utilityType, key);
        }

        public void UnRegister<T>() where T : class, IContainable
        {
            if (mContainableList.Contains(typeof(T)))
            {
                mContainableList.Remove(typeof(T));
            }
            ContainerManager.Instance.UnRegister<T>();
        }

        public void UnRegister(object key)
        {
            if (mContainableList.Contains(key))
            {
                mContainableList.Remove(key);
            }
            ContainerManager.Instance.UnRegister(key);
        }
        #endregion

        private void UnRegisterWhenSceneExit()
        {
            for (int i = mContainableList.Count - 1; i >= 0; i--)
            {
                UnRegister(mContainableList[i]);
            }
        }
    }
}
