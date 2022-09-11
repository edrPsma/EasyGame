using EG.Resource.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource
{
    public partial class ResourcesManager
    {
        #region 预加载
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="clear">跳场景是否清空缓存</param>
        public void Preload(string path, bool clear = false, bool isSprite = false)
        {
            if (string.IsNullOrEmpty(path)) return;

            uint crc = CRC32.GetCRC32(path);
            ResourcesItem item = GetResourceItem(crc, clear, 0);
            //如果有缓存,则从缓存中加载
            if (item != null)
            {
                return;
            }

            UnityEngine.Object result = null;
#if UNITY_EDITOR
            //从编辑器中加载
            if (!LoadFromAB)
            {
                if (isSprite)
                {
                    result = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
                else
                {
                    result = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
                }
                if (result == null)
                {
                    Debug.LogError("没有该路径,path:" + path);
                    return;
                }

                item = new ResourcesItem();
                item.CRC = crc;
                item.Object = result;
                item.Clear = clear;
                mAssetDic.Add(crc, item);
                mGuidDic.Add(result.GetInstanceID(), crc);
                return;
            }
#endif
            //从AB包中加载
            if (result == null)
            {
                ResourcesItem resItem = ABManager.Instance.LoadResourceAB(crc);
                if (resItem == null)
                {
                    Debug.LogError("没有该路径,path:" + path);
                    return;
                }

                if (isSprite)
                {
                    result = resItem.AssetBundle.LoadAsset<Sprite>(resItem.AssetName);
                }
                else
                {
                    result = resItem.AssetBundle.LoadAsset(resItem.AssetName);
                }
                resItem.Object = result;
                resItem.Clear = clear;

                //将资源添加进缓存
                mAssetDic.Add(crc, resItem);
                mGuidDic.Add(result.GetInstanceID(), crc);
            }
            return;
        }
        #endregion
    }
}
