using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource.Core
{
    public class PoolManager : Singleton<PoolManager>
    {
        // 对象池字典
        protected Dictionary<Type, object> mPoolDic = new Dictionary<Type, object>();

        #region 对象池
        public ObjectPool<T> GetOrCreatePool<T>(int maxCount) where T : class, new()
        {
            Type type = typeof(T);
            object result = mPoolDic.TryGet(type);
            if (result == null)
            {
                ObjectPool<T> newPool = new ObjectPool<T>(maxCount);
                mPoolDic.Add(type, newPool);
                return newPool;
            }

            return result as ObjectPool<T>;
        }
        #endregion
    }
}
