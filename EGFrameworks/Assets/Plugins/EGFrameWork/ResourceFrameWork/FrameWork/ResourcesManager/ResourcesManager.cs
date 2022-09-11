using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Resource.Core;
using EG.Core;

namespace EG.Resource
{
    public partial class ResourcesManager : AbstractUtility
    {
        public bool LoadFromAB { get; set; } = true;
        // 缓存正在使用的资源
        public Dictionary<uint, ResourcesItem> mAssetDic { get; private set; } = new Dictionary<uint, ResourcesItem>();
        // 通过GUID标记正在使用的资源
        Dictionary<int, uint> mGuidDic = new Dictionary<int, uint>();
        // 异步加载驱动器
        //MonoBehaviour mAsyncDriver;
        // 异步加载队列
        List<AsyncItem>[] mLoadingAssetList = new List<AsyncItem>[3];
        // 正在异步加载的字典
        Dictionary<uint, AsyncItem> mLoadingAssetDic = new Dictionary<uint, AsyncItem>();
        // 异步加载项对象池
        ObjectPool<AsyncItem> mAsyncItemPool = PoolManager.Instance.GetOrCreatePool<AsyncItem>(50);
        ObjectPool<AsyncCallBack> mCallBackPool = PoolManager.Instance.GetOrCreatePool<AsyncCallBack>(100);
        // 异步加载最长等待时间
        private const long MAXLOADRESTIME = 200000;

        protected override void OnInit()
        {
            if (EGrameWork.Model == StartModel.Editor)
            {
                LoadFromAB = false;
            }

            if (LoadFromAB)
                ABManager.Instance.Init();

            for (int i = 0; i < mLoadingAssetList.Length; i++)
            {
                mLoadingAssetList[i] = new List<AsyncItem>();
            }

            StartCoroutine(AsyncLoader());
        }

        #region 取出资源并增加引用数
        // 取出资源并增加引用数
        ResourcesItem GetResourceItem(uint crc, bool clear = true, int refCount = 1)
        {
            ResourcesItem item = mAssetDic.TryGet(crc);
            if (item != null)
            {
                item.Clear = clear;
                item.RefCount += refCount;
            }
            return item;
        }
        #endregion

        #region 根据CRC增加引用
        public void AddRefCount(uint crc, int refCount = 1)
        {
            ResourcesItem item = mAssetDic.TryGet(crc);
            if (item != null)
            {
                item.RefCount += refCount;
            }
        }
        #endregion

        #region 清除缓存
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            List<uint> clearList = new List<uint>();
            foreach (var temp in mGuidDic)
            {
                if (mAssetDic[temp.Value].Clear)
                {
                    clearList.Add(temp.Value);
                }
            }
            foreach (var temp in clearList)
            {
                ResourcesItem item = mAssetDic[temp];
                item.RefCount = 0;//强制回收所有引用
                UnLoad(item.Object, true);
            }
        }
        #endregion
    }
}
