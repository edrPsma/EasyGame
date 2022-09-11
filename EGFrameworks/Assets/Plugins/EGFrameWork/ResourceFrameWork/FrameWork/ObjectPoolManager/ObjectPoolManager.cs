using System.Collections.Generic;
using System;
using EG.Resource.Core;
using UnityEngine;

namespace EG.Resource
{
    public partial class ObjectPoolManager : AbstractUtility
    {
        // 回收节点
        public Transform RecycleNode { get; set; }
        // 场景节点
        public Transform SceneNode { get; set; }
        // 游戏物体项资源池
        protected ObjectPool<ObjectItem> mGameObjectItemPool = PoolManager.Instance.GetOrCreatePool<ObjectItem>(500);
        // 游戏物体项资源池字典
        protected Dictionary<uint, List<ObjectItem>> mGameObjectPoolDic = new Dictionary<uint, List<ObjectItem>>();
        // 游戏物体项字典,key为GUID
        protected Dictionary<int, ObjectItem> mGuidDic = new Dictionary<int, ObjectItem>();

        protected override void OnInit()
        {
            GameObject sceneNode = new GameObject("SceneNode");
            sceneNode.SetActive(false);
            SceneNode = sceneNode.transform;
            SceneNode.SetParent(transform);
            GameObject recycleNode = new GameObject("RecycleNode");
            recycleNode.SetActive(false);
            RecycleNode = recycleNode.transform;
            RecycleNode.SetParent(transform);
        }

        public void Init(Transform sceneNode, Transform recycleNode)
        {
            RecycleNode = recycleNode;
            SceneNode = sceneNode;
        }

        #region 从对象池获取游戏物体项
        ObjectItem GetFromPool(uint crc)
        {
            ObjectItem item = null;
            List<ObjectItem> itemList = mGameObjectPoolDic.TryGet(crc);
            if (itemList != null && itemList.Count > 0)
            {
                item = itemList[0];
                itemList.RemoveAt(0);
            }
            return item;
        }
        #endregion

        #region 清空缓存
        public void ClearCache()
        {
            List<int> clearList = new List<int>();
            foreach (var temp in mGuidDic)
            {
                if (temp.Value.Clear)
                {
                    clearList.Add(temp.Key);
                }
            }

            foreach (var temp in clearList)
            {
                UnLoad(mGuidDic[temp].GameObject, false, true);
            }
        }
        #endregion
    }
}
