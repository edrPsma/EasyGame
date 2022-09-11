using System.Collections.Generic;
using UnityEngine;

namespace EG.Core
{
    public class Container
    {
        Dictionary<object, IContainable> mInstances = new Dictionary<object, IContainable>();

        #region 注册
        /// <summary>
        /// 注册容器内容
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="instance">实例</param>
        public void Register<T>(T instance) where T : IContainable
        {
            var key = typeof(T);

            if (mInstances.ContainsKey(key))
            {
                mInstances[key] = instance;
            }
            else
            {
                mInstances.Add(key, instance);
                instance.Init();
            }
        }

        /// <summary>
        /// 注册容器内容
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public bool Register(object key, IContainable instance)
        {
            if (instance == null)
            {
                Debug.LogError(key + "指向的实例为空");
                return false;
            }

            var target = instance as IContainable;
            if (target == null)
            {
                Debug.LogError(instance.GetType() + "没有直接或间接实现IContainable接口");
                return false;
            }

            if (mInstances.ContainsKey(key))
            {
                mInstances[key] = target;
            }
            else
            {
                mInstances.Add(key, target);
                target.Init();
            }
            return true;
        }
        #endregion

        #region 获取容器内容
        /// <summary>
        /// 获取容器内容
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>系统或者数据</returns>
        public T Get<T>() where T : class, IContainable
        {
            var key = typeof(T);

            if (mInstances.ContainsKey(key))
            {
                return mInstances[key] as T;
            }

            return null;
        }

        public IContainable Get(object key)
        {
            if (mInstances.ContainsKey(key))
            {
                return mInstances[key];
            }

            return null;
        }
        #endregion

        #region 注销容器内容
        /// <summary>
        /// 注销容器内容
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>是否注销成功</returns>
        public bool UnRegister<T>() where T : IContainable
        {
            var key = typeof(T);
            if (mInstances.ContainsKey(key))
            {
                mInstances.Remove(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 注销容器内容
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>注销是否成功</returns>
        public bool UnRegister(object key)
        {
            if (mInstances.ContainsKey(key))
            {
                mInstances.Remove(key);
                return true;
            }
            return false;
        }
        #endregion
    }
}
