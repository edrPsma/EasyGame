using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EG.Resource.Core
{
    public class ABManager : Singleton<ABManager>
    {
        //所有资源项,key为配置文件crc(根据路径获得),value为资源项
        Dictionary<uint, ResourcesItem> mAllRourceItemDic = new Dictionary<uint, ResourcesItem>();
        //已加载的AB包,key为ab包名,value为AB包项
        Dictionary<string, ABItem> mABItemDic = new Dictionary<string, ABItem>();
        //ABItem对象池
        ObjectPool<ABItem> mABItemPool = PoolManager.Instance.GetOrCreatePool<ABItem>(500);

        #region 加载配置表
        /// <summary>
        /// 初始化,加载配置表
        /// </summary>
        public void Init()
        {
            BuildingConfig buildingConfig = Resources.Load<BuildingConfig>("ABConfig");
            var abConfigName = buildingConfig.ConfigurationFileABPackageName;
            Resources.UnloadAsset(buildingConfig);
            string configPath = Application.streamingAssetsPath + "/" + abConfigName;
            AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
            TextAsset textAsset = configAB.LoadAsset<TextAsset>("AssetBundleConfig");

            if (textAsset == null)
            {
                Debug.LogError("找不到配置文件,path:" + configPath);
                return;
            }

            //读取2进制配置文件信息
            MemoryStream stream = new MemoryStream(textAsset.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            ABConfig config = (ABConfig)bf.Deserialize(stream);
            stream.Close();

            foreach (ABBase abBase in config.ABList)
            {
                ResourcesItem item = new ResourcesItem();
                item.CRC = abBase.Crc;
                item.AssetName = abBase.AssetName;
                item.ABName = abBase.ABName;
                item.ABDependce = abBase.ABDependce;
                if (mAllRourceItemDic.ContainsKey(item.CRC))
                {
                    Debug.LogError("相同的CRC:" + item.CRC + ",请检查配置文件");
                    continue;
                }
                mAllRourceItemDic.Add(item.CRC, item);
            }
        }
        #endregion

        #region 加载资源项
        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="crc">crc</param>
        /// <returns>资源项</returns>
        public ResourcesItem LoadResourceAB(uint crc)
        {
            ResourcesItem item = mAllRourceItemDic.TryGet(crc);

            if (item == null)
            {
                Debug.LogError("没有找到CRC:" + crc);
                return null;
            }

            //如果AB包已经加载,直接返回
            if (item.AssetBundle != null)
            {
                return item;
            }

            //AB包还没有加载,加载AB包
            foreach (var denpence in item.ABDependce)
            {
                LoadAb(denpence);//加载依赖
            }
            item.AssetBundle = LoadAb(item.ABName);

            return item;
        }

        //根据AB包名加载AB包
        AssetBundle LoadAb(string abName)
        {
            ABItem abItem = mABItemDic.TryGet(abName);

            //如果AB包已经加载,直接返回
            if (abItem != null)
            {
                abItem.RefCount++;
                return abItem.AssetBundle;
            }

            //如果AB包没有加载,则加载
            AssetBundle assetBundle = null;
            string fullPath = Application.streamingAssetsPath + "/" + abName;
            assetBundle = AssetBundle.LoadFromFile(fullPath);

            if (assetBundle == null)
            {
                Debug.LogError("没有找到该路径" + fullPath);
            }

            abItem = mABItemPool.Spawn(true);//从对象池中取出一个ABItem
            abItem.AssetBundle = assetBundle;
            abItem.RefCount++;
            mABItemDic.Add(abName, abItem);
            return abItem.AssetBundle;
        }
        #endregion

        #region 卸载资源项
        /// <summary>
        /// 卸载资源项
        /// </summary>
        /// <param name="item">资源项</param>
        public void UnLoadResourceAb(ResourcesItem item)
        {
            if (item == null) return;

            item.AssetBundle = null;
            if (item.ABDependce != null && item.ABDependce.Count > 0)
            {
                for (int i = 0; i < item.ABDependce.Count; i++)
                {
                    UnloadAB(item.ABDependce[i]);
                }
            }
            UnloadAB(item.ABName);
        }

        void UnloadAB(string abName)
        {
            ABItem abItem = mABItemDic.TryGet(abName);

            if (abItem != null)
            {
                abItem.RefCount--;
                if (abItem.RefCount <= 0 && abItem.AssetBundle != null)
                {
                    abItem.AssetBundle.Unload(true);
                    abItem.Reset();
                    mABItemPool.Recycle(abItem);
                    mABItemDic.Remove(abName);
                }
            }
        }
        #endregion

        #region 查找AB包
        public ResourcesItem FindResourceItem(uint crc)
        {
            ResourcesItem result = mAllRourceItemDic.TryGet(crc);
            return result;
        }
        #endregion
    }
}
