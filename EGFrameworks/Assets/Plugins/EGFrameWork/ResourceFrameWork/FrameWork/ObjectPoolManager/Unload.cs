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
        /// 卸载游戏物体
        /// </summary>
        /// <param name="gameObject">目标</param>
        /// <param name="recycle">是否回收到对象池</param>
        /// <param name="destoryCache">不会收的话是否清除缓存</param>
        public void UnLoad(GameObject gameObject, bool recycle = true, bool destoryCache = false)
        {
            int tempId = gameObject.GetInstanceID();

            if (!mGuidDic.ContainsKey(tempId))
            {
                Debug.LogError("该游戏物体不是由ObjectPoolManager创建的,回收失败,name:" + gameObject.name);
                return;
            }

            ObjectItem objectItem = mGuidDic[tempId];
            // 如果不回收,直接删除游戏物体
            if (!recycle)
            {
                uint crc = objectItem.CRC;
                GameObject.Destroy(gameObject);

                objectItem.Reset();
                mGameObjectItemPool.Recycle(objectItem);

                // 删除缓存
                mGuidDic.Remove(tempId);
                int count = 0;
                foreach (var temp in mGuidDic.Values)
                {
                    if (crc == temp.CRC)
                    {
                        count++;
                        break;
                    }
                }
                if (count == 0)//已经没有缓存该物体,删除资源池缓存
                {
                    mGameObjectPoolDic.Remove(crc);
                    // 只有当没有任何地方缓存该游戏物体时,才能删除资源缓存
                    ContainerManager.Instance.GetUtility<ResourcesManager>().UnLoad(crc, destoryCache);
                }
                else
                {
                    ContainerManager.Instance.GetUtility<ResourcesManager>().UnLoad(crc, false);
                }
                return;
            }

            // 如果回收
            if (objectItem.IsInPool)
            {
                Debug.LogError("重复回收,对象已经在池中,name:" + objectItem.GameObject.name);
                return;
            }
            //获取复位器进行复位
            foreach (var restorer in objectItem.GameObject.GetComponents<IResettable>())
            {
                restorer.Reset();
            }
            List<ObjectItem> objectItemList = mGameObjectPoolDic[objectItem.CRC];
            objectItemList.Add(objectItem);
            objectItem.GameObject.transform.SetParent(RecycleNode, false);
#if UNITY_EDITOR
            objectItem.GameObject.name += "(Recycle)";
#endif
            objectItem.IsInPool = true;
            ContainerManager.Instance.GetUtility<ResourcesManager>().UnLoad(objectItem.CRC, false);
        }
    }
}
