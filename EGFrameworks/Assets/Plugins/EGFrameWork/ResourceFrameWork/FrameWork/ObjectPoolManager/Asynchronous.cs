using EG.Core;
using EG.Resource.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource
{
    public delegate void GameObjectAsyncCompleted(string path, GameObject gameObject, params object[] paramList);

    public partial class ObjectPoolManager
    {
        /// <summary>
        /// 异步加载游戏物体
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="onComplete">回调</param>
        /// <param name="priority">加载优先级</param>
        /// <param name="parent">父物体</param>
        /// <param name="clear">跳场景是否清空缓存</param>
        /// <param name="paramList">参数列表</param>
        public void InstantiateObjectAsync(string path, GameObjectAsyncCompleted onComplete, LoadPriority priority, Transform parent = null, bool clear = true, params object[] paramList)
        {
            if (string.IsNullOrEmpty(path)) return;

            uint crc = CRC32.GetCRC32(path);
            ObjectItem objectItem = GetFromPool(crc);
            // 如果资源池里有,则直接返回,并且引用数+1
            if (objectItem != null)
            {
                var result = objectItem.GameObject;
                if (parent != null)
                {
                    result.transform.SetParent(parent, false);
                }
                else
                {
                    result.transform.SetParent(SceneNode, false);
                }
                objectItem.IsInPool = false;
#if UNITY_EDITOR
                result.name = result.name.Replace("(Recycle)", "");
#endif
                ContainerManager.Instance.GetUtility<ResourcesManager>().AddRefCount(crc);
                onComplete?.Invoke(path, result, paramList);
            }

            // 如果没有,则加载
            object[] objects = new object[4];
            objects[0] = parent;
            objects[1] = clear;
            objects[2] = onComplete;
            objects[3] = paramList;
            ContainerManager.Instance.GetUtility<ResourcesManager>().LoadAsync(path, OnComplete, priority, false, clear, objects);
        }

        void OnComplete(string path, Object asset, params object[] paramList)
        {
            if (asset == null)
            {
                Debug.LogError("资源加载失败,path:" + path);
                return;
            }

            uint crc = CRC32.GetCRC32(path);
            List<ObjectItem> objectItemList = null;
            if (!mGameObjectPoolDic.ContainsKey(crc))// 如果一次都没加载过
            {
                objectItemList = new List<ObjectItem>();
                mGameObjectPoolDic.Add(crc, objectItemList);
            }
            objectItemList = mGameObjectPoolDic[crc];

            GameObject gameObject = GameObject.Instantiate(asset as GameObject);
            if (paramList[0] != null)
            {
                gameObject.transform.SetParent(paramList[0] as Transform, false);
            }
            else
            {
                gameObject.transform.SetParent(SceneNode, false);
            }
            // 添加缓存
            ObjectItem objectItem = mGameObjectItemPool.Spawn(true);
            objectItem.CRC = crc;
            objectItem.Clear = (bool)paramList[1];
            objectItem.GameObject = gameObject;
            objectItem.GUID = gameObject.GetInstanceID();
            objectItem.IsInPool = false;
            mGuidDic.Add(objectItem.GUID, objectItem);

            (paramList[2] as GameObjectAsyncCompleted)?.Invoke(path, gameObject, paramList[3]);
        }
    }
}
