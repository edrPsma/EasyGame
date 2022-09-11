using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using EG.Resource;
using EG.Scene;

namespace EG.Core
{
    public class ContainerManager
    {
        #region 单例
        private Container mContainer = new Container();
        private static ContainerManager mInstance = null;
        public static ContainerManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new ContainerManager();
                }
                return mInstance;
            }
        }
        #endregion

        public Transform SystemNode { get; private set; }
        public Transform ModelNode { get; private set; }
        public Transform UtilityNode { get; private set; }

        List<object> mModelList = new List<object>();
        List<object> mSystemList = new List<object>();
        List<object> mUtilityList = new List<object>();

        public void Init(Action beforeInit = null)
        {
            CreateParentNode();
            beforeInit?.Invoke();

            // 获取所有IContainable类
            Dictionary<Type, Assembly> allContainables = GetAllContainable();
            // 扫描所有标记类，key为容器key,value为实例
            Dictionary<object, IContainable> utilitys = ContainableScanner.ScanningUtility(allContainables);
            Dictionary<object, IContainable> models = ContainableScanner.ScanningModel(allContainables);
            Dictionary<object, IContainable> systems = ContainableScanner.ScanningSystem(allContainables);

            // 可能有后续操作

            // 将标记类的实例添加进容器
            AddToContainer(utilitys, mUtilityList);
            AddToContainer(models, mModelList);
            AddToContainer(systems, mSystemList);
        }

        void CreateParentNode()
        {
            GameObject system = new GameObject("System");
            SystemNode = system.transform;
            GameObject.DontDestroyOnLoad(system);
            GameObject model = new GameObject("Model");
            ModelNode = model.transform;
            GameObject.DontDestroyOnLoad(model);
            GameObject utility = new GameObject("Utility");
            UtilityNode = utility.transform;
            GameObject.DontDestroyOnLoad(utility);
        }

        Dictionary<Type, Assembly> GetAllContainable()
        {
            Dictionary<Type, Assembly> result = new Dictionary<Type, Assembly>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblys)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (!typeof(IContainable).IsAssignableFrom(type)) continue;

                    result.Add(type, assembly);
                }
            }
            return result;
        }

        void AddToContainer(Dictionary<object, IContainable> containables, List<object> list)
        {
            foreach (var containable in containables)
            {
                mContainer.Register(containable.Key, containable.Value);
                list.Add(containable.Key);
            }
        }

        #region 获取容器对象
        public T2 GetSystem<T2>() where T2 : class, ISystem
        {
            var type = typeof(T2);
            if (mSystemList.Contains(type))
            {
                return mContainer.Get<T2>();
            }
            return null;
        }
        public object GetSystem(object key)
        {
            if (mSystemList.Contains(key))
            {
                return mContainer.Get(key);
            }
            return null;
        }

        public T2 GetModel<T2>() where T2 : class, IModel
        {
            var type = typeof(T2);
            if (mModelList.Contains(type))
            {
                return mContainer.Get<T2>();
            }
            return null;
        }
        public object GetModel(object key)
        {
            if (mModelList.Contains(key))
            {
                return mContainer.Get(key);
            }
            return null;
        }

        public T2 GetUtility<T2>() where T2 : class, IUtility
        {
            var type = typeof(T2);
            if (mUtilityList.Contains(type))
            {
                return mContainer.Get<T2>();
            }
            return null;
        }
        public object GetUtility(object key)
        {
            if (mUtilityList.Contains(key))
            {
                return mContainer.Get(key);
            }
            return null;
        }
        #endregion

        #region 注册容器对象
        public void RegisterSystem<T2>(object key = null) where T2 : class, ISystem
        {
            GameObject obj = null;
            if (key == null)
            {
                obj = new GameObject(typeof(T2).Name);
                mSystemList.Add(typeof(T2));
            }
            else
            {
                obj = new GameObject(key.ToString());
                mSystemList.Add(key);
            }
            obj.transform.SetParent(SystemNode);
            var instance = obj.AddComponent(typeof(T2));
            mContainer.Register<T2>(instance as T2);
        }
        public void RegisterSystem(Type systemType, object key = null)
        {
            GameObject obj = null;
            if (key == null)
            {
                obj = new GameObject(systemType.Name);
                mModelList.Add(systemType);
            }
            else
            {
                obj = new GameObject(key.ToString());
                mModelList.Add(key);
            }
            obj.transform.SetParent(SystemNode);
            var instance = obj.AddComponent(systemType);
            mContainer.Register(instance as IContainable);
        }

        public void RegisterModel<T2>(object key = null) where T2 : class, IModel
        {
            GameObject obj = null;
            if (key == null)
            {
                obj = new GameObject(typeof(T2).Name);
                mModelList.Add(typeof(T2));
            }
            else
            {
                obj = new GameObject(key.ToString());
                mModelList.Add(key);
            }
            obj.transform.SetParent(ModelNode);
            var instance = obj.AddComponent(typeof(T2));
            mContainer.Register<T2>(instance as T2);
        }
        public void RegisterModel(Type modelType, object key = null)
        {
            GameObject obj = null;
            if (key == null)
            {
                obj = new GameObject(modelType.Name);
                mModelList.Add(modelType);
            }
            else
            {
                obj = new GameObject(key.ToString());
                mModelList.Add(key);
            }
            obj.transform.SetParent(ModelNode);
            var instance = obj.AddComponent(modelType);
            mContainer.Register(instance as IContainable);
        }

        public void RegisterUtility<T2>(object key = null) where T2 : class, IUtility
        {
            GameObject obj = null;
            if (key == null)
            {
                obj = new GameObject(typeof(T2).Name);
                mUtilityList.Add(typeof(T2));
            }
            else
            {
                obj = new GameObject(key.ToString());
                mUtilityList.Add(key);
            }
            obj.transform.SetParent(UtilityNode);
            var instance = obj.AddComponent(typeof(T2));
            mContainer.Register<T2>(instance as T2);
        }
        public void RegisterUtility(Type utilitType, object key = null)
        {
            GameObject obj = null;
            if (key == null)
            {
                obj = new GameObject(utilitType.Name);
                mUtilityList.Add(utilitType);
            }
            else
            {
                obj = new GameObject(key.ToString());
                mUtilityList.Add(key);
            }
            obj.transform.SetParent(UtilityNode);
            var instance = obj.AddComponent(utilitType);
            mContainer.Register(instance as IContainable);
        }
        #endregion

        #region 容器对象注销
        public void UnRegister<T2>() where T2 : class, IContainable
        {
            var type = typeof(T2);
            if (typeof(ISystem).IsAssignableFrom(type) && mSystemList.Contains(type))
            {
                mSystemList.Remove(type);
            }
            else if (typeof(IModel).IsAssignableFrom(type) && mModelList.Contains(type))
            {
                mModelList.Remove(type);
            }
            else if (typeof(IUtility).IsAssignableFrom(type) && mUtilityList.Contains(type))
            {
                mUtilityList.Remove(type);
            }
            MonoBehaviour target = mContainer.Get<T2>() as MonoBehaviour;
            mContainer.UnRegister<T2>();
            GameObject.Destroy(target);
        }

        public void UnRegister(object key)
        {
            if (mSystemList.Contains(key))
            {
                mSystemList.Remove(key);
            }
            else if (mModelList.Contains(key))
            {
                mModelList.Remove(key);
            }
            else if (mUtilityList.Contains(key))
            {
                mUtilityList.Remove(key);
            }
            MonoBehaviour target = mContainer.Get(key) as MonoBehaviour;
            mContainer.UnRegister(key);
            GameObject.Destroy(target.gameObject);
        }
        #endregion

    }
}
