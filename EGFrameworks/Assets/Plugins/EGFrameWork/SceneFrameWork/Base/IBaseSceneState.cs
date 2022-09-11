using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Core;
using System;

namespace EG.Scene.Core
{
    public interface IBaseSceneState
    {
        string ScenePath { get; set; }

        void LoadStart();
        void EnterScene();
        void ExitScene();
        void RegisterSystem<T>(object key, bool unRegisterWhenSceneExit) where T : class, ISystem;
        void RegisterSystem(Type systemType, object key, bool unRegisterWhenSceneExit);
        void RegisterModel<T>(object key, bool unRegisterWhenSceneExit) where T : class, IModel;
        void RegisterModel(Type modelType, object key, bool unRegisterWhenSceneExit);
        void RegisterUtility<T>(object key, bool unRegisterWhenSceneExit) where T : class, IUtility;
        void RegisterUtility(Type utilityType, object key, bool unRegisterWhenSceneExit);
        void UnRegister<T>() where T : class, IContainable;
        void UnRegister(object key);
        /// <summary>
        /// 获取系统层对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetSystem<T>() where T : class, ISystem;
        /// <summary>
        /// 获取系统层对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetSystem(object key);
        /// <summary>
        /// 获取数据层对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetModel<T>() where T : class, IModel;
        /// <summary>
        /// 获取数据层对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetModel(object key);
        /// <summary>
        /// 获取工具层对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetUtility<T>() where T : class, IUtility;
        /// <summary>
        /// 获取工具层对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetUtility(object key);
    }

}
