using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Resource.Core;

namespace EG.Resource
{
    /// <summary>
    /// 异步加载回调委托
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="asset">加载出来的资源</param>
    /// <param name="paramList">参数列表</param>
    public delegate void AsyncCompleted(string path, Object asset, params object[] paramList);


    public partial class ResourcesManager
    {
        #region 异步加载
        /// <summary>
        /// 异步资源加载
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="onComplete">回调</param>
        /// <param name="priority">加载优先级</param>
        /// <param name="isSprite">是否是图片</param>
        /// <param name="clear">跳场景是否清空缓存</param>
        /// <param name="paramList">参数列表</param>
        public void LoadAsync(string path, AsyncCompleted onComplete, LoadPriority priority, bool isSprite = false, bool clear = true, params object[] paramList)
        {
            if (string.IsNullOrEmpty(path)) return;

            uint crc = CRC32.GetCRC32(path);
            ResourcesItem item = GetResourceItem(crc, clear);

            // 如果已经加载,则直接执行回调
            if (item != null)
            {
                onComplete?.Invoke(path, item.Object, paramList);
                return;
            }

            AsyncItem asyncItem = mLoadingAssetDic.TryGet(crc);
            AsyncCallBack callBack = mCallBackPool.Spawn(true);
            callBack.OnCompleted = onComplete;
            callBack.ParamList = paramList;
            // 如果不在异步加载队列中,则添加
            if (asyncItem == null)
            {
                asyncItem = mAsyncItemPool.Spawn(true);
                asyncItem.CRC = crc;
                asyncItem.LoadPriority = priority;
                asyncItem.Path = path;
                asyncItem.IsSprite = isSprite;
                asyncItem.Clear = clear;
                mLoadingAssetDic.Add(crc, asyncItem);
                mLoadingAssetList[(int)priority].Add(asyncItem);
            }

            asyncItem.CallBackList.Add(callBack);
        }
        #endregion

        #region 异步加载协程
        IEnumerator AsyncLoader()
        {
            List<AsyncCallBack> callBackList = null;

            //上一次yield的时间
            long lastYiledTime = System.DateTime.Now.Ticks;
            while (true)
            {
                bool haveYield = false;
                for (int i = 0; i < 3; i++)
                {
                    if (mLoadingAssetList[0].Count > 0)
                    {
                        i = 0;// 如果高优先级队列没有加载完,则继续加载高优先级队列
                    }
                    else if (mLoadingAssetList[1].Count > 0)
                    {
                        i = 1;// 如果中优先级队列没有加载完,则继续加载中优先级队列
                    }

                    List<AsyncItem> asyncItemList = mLoadingAssetList[i];
                    if (asyncItemList.Count <= 0) continue;

                    AsyncItem asyncItem = asyncItemList[0];
                    asyncItemList.RemoveAt(0);
                    callBackList = asyncItem.CallBackList;
                    Object obj = null;
                    ResourcesItem item = null;

#if UNITY_EDITOR
                    if (!LoadFromAB)
                    {
                        if (asyncItem.IsSprite)
                            obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(asyncItem.Path);
                        else
                            obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(asyncItem.Path);
                        //模拟异步加载
                        yield return new WaitForSeconds(0.3f);

                        //item = ABManager.Instance.FindResourceItem(loadingItem.Crc);
                        item = new ResourcesItem();
                        item.CRC = asyncItem.CRC;
                        item.Object = obj;
                        item.Clear = asyncItem.Clear;
                    }
#endif

                    if (obj == null)
                    {
                        item = ABManager.Instance.LoadResourceAB(asyncItem.CRC);
                        if (item != null && item.AssetBundle != null)
                        {
                            AssetBundleRequest abRequest = null;
                            if (asyncItem.IsSprite)
                            {
                                abRequest = item.AssetBundle.LoadAssetAsync<Sprite>(item.AssetName);
                            }
                            else
                            {
                                abRequest = item.AssetBundle.LoadAssetAsync(item.AssetName);
                            }
                            yield return abRequest;
                            if (abRequest.isDone)
                            {
                                obj = abRequest.asset;
                                item.Object = obj;
                                item.Clear = asyncItem.Clear;
                            }
                            lastYiledTime = System.DateTime.Now.Ticks;
                        }
                    }

                    // 加入缓存
                    mAssetDic.Add(asyncItem.CRC, item);
                    int guid = item.Object.GetInstanceID();
                    mGuidDic.Add(guid, item.CRC);
                    // 添加引用数,即回调数
                    item.RefCount += asyncItem.CallBackList.Count;

                    // 执行回调
                    for (int j = 0; j < callBackList.Count; j++)
                    {
                        AsyncCallBack callBack = callBackList[j];

                        if (callBack != null && callBack.OnCompleted != null)
                        {
                            callBack.OnCompleted(asyncItem.Path, obj, callBack.ParamList);
                            callBack.OnCompleted = null;
                        }
                        callBack.Reset();
                        mCallBackPool.Recycle(callBack);
                    }

                    obj = null;
                    callBackList.Clear();
                    mLoadingAssetDic.Remove(asyncItem.CRC);

                    asyncItem.Reset();
                    mAsyncItemPool.Recycle(asyncItem);

                    if (System.DateTime.Now.Ticks - lastYiledTime > MAXLOADRESTIME)
                    {
                        yield return null;
                        lastYiledTime = System.DateTime.Now.Ticks;
                        haveYield = true;
                    }
                }

                if (System.DateTime.Now.Ticks - lastYiledTime > MAXLOADRESTIME || !haveYield)
                {
                    lastYiledTime = System.DateTime.Now.Ticks;
                    yield return null;
                }
            }
        }
        #endregion
    }
}
