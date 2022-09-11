using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Resource.Core;
using EG.Core;

namespace EG.Resource
{
    public partial class ObjectPoolManager
    {
        /// <summary>
        /// 同步加载游戏物体
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="parent">父物体</param>
        /// <param name="clear">跳场景是否清空缓存</param>
        /// <returns></returns>
        public GameObject InstantiateObject(string path, Transform parent = null, bool clear = true)
        {
            if (string.IsNullOrEmpty(path)) return null;

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
                return result;
            }

            // 如果资源池内没有,则加载
            GameObject obj = ContainerManager.Instance.GetUtility<ResourcesManager>().Load<GameObject>(path);

            if (obj == null) return null;

            List<ObjectItem> objectItemList = null;
            if (!mGameObjectPoolDic.ContainsKey(crc))// 如果一次都没加载过
            {
                objectItemList = new List<ObjectItem>();
                mGameObjectPoolDic.Add(crc, objectItemList);
            }

            objectItemList = mGameObjectPoolDic[crc];

            GameObject gameObject = GameObject.Instantiate(obj);
            if (parent != null)
            {
                gameObject.transform.SetParent(parent, false);
            }
            else
            {
                gameObject.transform.SetParent(SceneNode, false);
            }
            // 添加缓存
            objectItem = mGameObjectItemPool.Spawn(true);
            objectItem.CRC = crc;
            objectItem.Clear = clear;
            objectItem.GameObject = gameObject;
            objectItem.GUID = gameObject.GetInstanceID();
            objectItem.IsInPool = false;
            mGuidDic.Add(objectItem.GUID, objectItem);

            return gameObject;
        }
    }
}
