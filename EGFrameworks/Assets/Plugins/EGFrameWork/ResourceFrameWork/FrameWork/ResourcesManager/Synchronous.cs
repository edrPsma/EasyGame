using UnityEngine;
using EG.Resource.Core;

namespace EG.Resource
{
    public partial class ResourcesManager
    {
        #region 同步加载
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">路径</param>
        /// <param name="clear">跳场景是否清空缓存</param>
        /// <returns></returns>
        public T Load<T>(string path, bool clear = true) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path)) return null;

            uint crc = CRC32.GetCRC32(path);
            ResourcesItem item = GetResourceItem(crc, clear);
            //如果有缓存,则从缓存中加载
            if (item != null)
            {
                return item.Object as T;
            }


            UnityEngine.Object result = null;
#if UNITY_EDITOR
            //从编辑器中加载
            if (!LoadFromAB)
            {
                if (typeof(T) == typeof(Sprite))
                {
                    result = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
                else
                {
                    result = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
                }
                //result = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
                if (result == null)
                {
                    Debug.LogError("没有该路径,path:" + path);
                    return null;
                }

                item = new ResourcesItem();
                item.CRC = crc;
                item.Object = result;
                item.RefCount++;// 添加引用
                item.Clear = clear;
                mAssetDic.Add(crc, item);
                mGuidDic.Add(result.GetInstanceID(), crc);
                return result as T;
            }
#endif
            //从AB包中加载
            if (result == null)
            {
                ResourcesItem resItem = ABManager.Instance.LoadResourceAB(crc);
                if (resItem == null)
                {
                    Debug.LogError("没有该路径,path:" + path);
                    return null;
                }

                if (typeof(T) == typeof(Sprite))
                {
                    result = resItem.AssetBundle.LoadAsset<Sprite>(resItem.AssetName);
                }
                else
                {
                    result = resItem.AssetBundle.LoadAsset(resItem.AssetName);
                }
                resItem.Object = result;
                resItem.RefCount++;// 添加引用
                resItem.Clear = clear;

                //将资源添加进缓存
                mAssetDic.Add(crc, resItem);
                mGuidDic.Add(result.GetInstanceID(), crc);
            }
            return result as T;
        }
        #endregion
    }
}
