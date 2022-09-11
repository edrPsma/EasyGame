using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Resource.Core;

namespace EG.Resource
{
    public partial class ResourcesManager
    {
        #region 资源卸载
        /// <summary>
        /// 资源卸载
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="destoryCache">是否清除缓存</param>
        public void UnLoad(Object target, bool destoryCache = true)
        {
            if (target == null) return;

            int guid = target.GetInstanceID();
            uint crc = mGuidDic.TryGet(guid);
            ResourcesItem item = mAssetDic.TryGet(crc);
            if (item == null)
            {
                Debug.LogError("没有找到该资源,请检查是否重复卸载");
                return;
            }

            item.RefCount--;
            if (item.RefCount > 0)
            {
                return;
            }

            if (!destoryCache)
            {
                return;
            }

            //如果引用数为0,卸载资源
            Resources.UnloadAsset(item.Object);
            if (LoadFromAB) ABManager.Instance.UnLoadResourceAb(item);
            item.Object = null;
            item.RefCount = 0;//重置引用数

            //清除缓存
            mAssetDic.Remove(crc);
            mGuidDic.Remove(guid);
        }

        public void UnLoad(uint crc, bool destoryCache)
        {
            ResourcesItem item = mAssetDic.TryGet(crc);
            if (item == null)
            {
                Debug.LogError("没有找到该资源,请检查是否重复卸载");
                return;
            }

            item.RefCount--;
            if (item.RefCount > 0)
            {
                return;
            }

            if (!destoryCache)
            {
                return;
            }

            //如果引用数为0,卸载资源
            if (item.Object.GetType() != typeof(GameObject))
            {
                Resources.UnloadAsset(item.Object);
            }
#if UNITY_EDITOR
            else
            {
                if (!LoadFromAB)
                    Resources.UnloadUnusedAssets();
            }
#endif
            if (LoadFromAB) ABManager.Instance.UnLoadResourceAb(item);
            item.Object = null;

            //清除缓存
            mAssetDic.Remove(crc);
            int guid = 0;
            foreach (var temp in mGuidDic)
            {
                if (crc == temp.Value)
                {
                    guid = temp.Key;
                }
            }
            mGuidDic.Remove(guid);
        }
        #endregion
    }
}
