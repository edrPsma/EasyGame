using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EG.Resource.Core
{
    public class ABBuilder
    {
        static string ABTargetPath = Application.dataPath + "/../AssetBundle";
        static string XMLPATH = Application.dataPath + "/AssetBundleConfig.xml";
        static string BYTEPATH = Application.dataPath + "/AssetBundleConfig.bytes";
        static string abconfigName;

        //k为包名,v为路径
        static Dictionary<string, string> allFileDir = new Dictionary<string, string>();
        //过滤的List,去除预制体中依赖的冗余的资源
        static List<string> allFileAB = new List<string>();
        //单个Prefab的AB包
        static Dictionary<string, List<string>> allPrefabDir = new Dictionary<string, List<string>>();
        //单个Scene的AB包
        static Dictionary<string, string> allSceneDir = new Dictionary<string, string>();
        //存储所有有效路径
        static List<string> allValidPaths = new List<string>();

        [MenuItem("EasyGame/2.打包AB包", false, 1)]
        public static void Build()
        {
            allFileDir.Clear();
            allFileAB.Clear();
            allPrefabDir.Clear();
            allSceneDir.Clear();
            allValidPaths.Clear();
            BuildingConfig buildingConfig = Resources.Load<BuildingConfig>("ABConfig");
            abconfigName = buildingConfig.ConfigurationFileABPackageName;
            if (!Directory.Exists(ABTargetPath))
            {
                Directory.CreateDirectory(ABTargetPath);
            }
            foreach (var item in buildingConfig.AllDirectoryPath)
            {
                if (allFileDir.ContainsKey(item.name))
                {
                    Debug.LogError("AB包配置文件重复,打包终止,name:" + item.name);
                    return;
                }
                else
                {
                    allFileDir.Add(item.name, item.path);
                    allFileAB.Add(item.path);
                    allValidPaths.Add(item.path);
                }
            }
            string[] allPrefabPath = buildingConfig.AllPrefabPath.ToArray();
            if (allPrefabPath.Length == 0)
            {
                allPrefabPath = new string[1];
                allPrefabPath[0] = "Assets";
            }
            string[] allStr = AssetDatabase.FindAssets("t:Prefab", allPrefabPath);
            for (int i = 0; i < allStr.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
                EditorUtility.DisplayProgressBar("查找预制体", "Prefab:" + path, i * 1.0f / allStr.Length);
                allValidPaths.Add(path);
                if (!ContainAllFileAB(path))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    string[] allDepend = AssetDatabase.GetDependencies(path);
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDepend.Length; j++)
                    {
                        if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                        {
                            allFileAB.Add(allDepend[j]);
                            allDependPath.Add(allDepend[j]);
                        }
                    }

                    if (allPrefabDir.ContainsKey(obj.name))
                    {
                        Debug.LogError("存在相同名字的Prefab,name:" + obj.name);
                    }
                    else
                    {
                        allPrefabDir.Add(obj.name, allDependPath);
                    }
                }
            }

            string[] allSceneStr = AssetDatabase.FindAssets("t:Scene", buildingConfig.AllScenePath.ToArray());
            for (int i = 0; i < allSceneStr.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allSceneStr[i]);
                EditorUtility.DisplayProgressBar("查找场景", "Scene:" + path, i * 1.0f / allSceneStr.Length);
                allValidPaths.Add(path);
                if (!ContainAllFileAB(path))
                {
                    var strs = path.Split('/');
                    var sceneName = strs[strs.Length - 1].Replace(".unity", "");
                    // string[] allDepend = AssetDatabase.GetDependencies(path);
                    // List<string> allDependPath = new List<string>();
                    // for (int j = 0; j < allDepend.Length; j++)
                    // {
                    //     if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                    //     {
                    //         allFileAB.Add(allDepend[j]);
                    //         allDependPath.Add(allDepend[j]);
                    //     }
                    // }

                    if (allSceneDir.ContainsKey(sceneName))
                    {
                        Debug.LogError("存在相同名字的Scene,name:" + sceneName);
                    }
                    else
                    {
                        allSceneDir.Add(sceneName, path);
                    }
                }
            }

            foreach (var name in allFileDir.Keys)
            {
                SetABName(name, allFileDir[name]);
            }

            foreach (var name in allPrefabDir.Keys)
            {
                SetABName(name, allPrefabDir[name]);
            }

            foreach (var name in allSceneDir.Keys)
            {
                SetABName(name, allSceneDir[name]);
            }
            Resources.UnloadAsset(buildingConfig);

            BuildAB();

            foreach (var name in allFileDir.Keys)
            {
                SetABName("", allFileDir[name]);
            }

            foreach (var name in allPrefabDir.Keys)
            {
                SetABName("", allPrefabDir[name]);
            }

            foreach (var name in allSceneDir.Keys)
            {
                SetABName("", allSceneDir[name]);
            }
            SetABName("", BYTEPATH.Replace(Application.dataPath, "Assets"));

            ClearABName();

            if (File.Exists(BYTEPATH))
            {
                File.Delete(BYTEPATH);
            }
            if (File.Exists(BYTEPATH + ".meta"))
            {
                File.Delete(BYTEPATH + ".meta");
            }
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }


        static bool ContainAllFileAB(string path)
        {
            for (int i = 0; i < allFileAB.Count; i++)
            {
                if (path == allFileAB[i] || path.Contains(allFileAB[i]) && (path.Replace(allFileAB[i], "")[0] == '/'))
                {
                    return true;
                }
            }

            return false;
        }

        #region 设置AB包名
        static void SetABName(string abName, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("不存在此路径,path:" + path);
                return;
            }
            else
            {
                assetImporter.assetBundleName = abName;
            }
        }
        static void SetABName(string abName, List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                SetABName(abName, paths[i]);
            }
        }
        #endregion

        #region 清除AB包名
        static void ClearABName()
        {
            string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < oldABNames.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
                EditorUtility.DisplayProgressBar("清除AB包名", "name:" + oldABNames[i], i * 1.0f / oldABNames.Length);
            }
        }
        #endregion

        #region 打包
        static void BuildAB()
        {
            string[] allBundes = AssetDatabase.GetAllAssetBundleNames();
            //k为全路径,v为包名
            Dictionary<string, string> resPathDic = new Dictionary<string, string>();
            for (int i = 0; i < allBundes.Length; i++)
            {
                string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundes[i]);
                for (int j = 0; j < allBundlePath.Length; j++)
                {
                    if (allBundlePath[j].EndsWith(".cs"))
                    {
                        continue;
                    }
                    //Debug.Log(allBundes[i] + "----" + allBundlePath[j]);
                    if (CheckValidPath(allBundlePath[j]))
                    {
                        resPathDic.Add(allBundlePath[j], allBundes[i]);
                    }
                }
            }

            DeleteUnUseAB();

            //生成XML配置文件和二进制文件
            CreateConfig(resPathDic);
            AssetDatabase.Refresh();

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles
            (
                ABTargetPath,
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget
            );

            if (manifest == null)
            {
                Debug.LogError("AB包打包失败");
            }
            else
            {
                Debug.Log("AB包打包成功");
            }
        }
        #endregion

        #region 生成配置表
        static void CreateConfig(Dictionary<string, string> resPathDic)
        {
            ABConfig config = new ABConfig();
            config.ABList = new List<ABBase>();
            foreach (var path in resPathDic.Keys)
            {
                ABBase abBase = new ABBase();
                abBase.Path = path;
                abBase.Crc = CRC32.GetCRC32(path);
                abBase.ABName = resPathDic[path];
                abBase.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);
                abBase.ABDependce = new List<string>();
                string[] resDependce = AssetDatabase.GetDependencies(path);
                for (int i = 0; i < resDependce.Length; i++)
                {
                    string tempPath = resDependce[i];
                    if (tempPath == path || path.EndsWith(".cs"))
                        continue;

                    string abName = "";
                    if (resPathDic.TryGetValue(tempPath, out abName))
                    {
                        if (abName == resPathDic[path])
                            continue;

                        if (!abBase.ABDependce.Contains(abName))
                        {
                            abBase.ABDependce.Add(abName);
                        }
                    }
                }
                config.ABList.Add(abBase);
            }
            if (File.Exists(XMLPATH)) File.Delete(XMLPATH);
            FileStream fileStream = new FileStream(XMLPATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(config.GetType());
            xs.Serialize(sw, config);
            sw.Close();
            fileStream.Close();

            foreach (var item in config.ABList)
            {
                item.Path = "";//因为二进制文件不需要Path,因此将其清除以减少内存
            }
            FileStream fs = new FileStream(BYTEPATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, config);
            fs.Close();
            AssetDatabase.ImportAsset(BYTEPATH.Replace(Application.dataPath, "Assets"));
            SetABName(abconfigName, BYTEPATH.Replace(Application.dataPath, "Assets"));
        }
        #endregion

        #region 是否有效路径
        static bool CheckValidPath(string path)
        {
            for (int i = 0; i < allValidPaths.Count; i++)
            {
                if (path.Contains(allValidPaths[i]))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 删除没用的AB包
        static void DeleteUnUseAB()
        {
            string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
            DirectoryInfo directoryInfo = new DirectoryInfo(ABTargetPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (ContainABName(file.Name, allBundlesName) || file.Name.EndsWith(".meta"))
                {
                    continue;
                }
                else
                {
                    if (file.Name == "AssetBundle" || file.Name == "AssetBundle.manifest")
                        continue;

                    if (file.Name == abconfigName || file.Name == abconfigName + ".manifest")
                        continue;

                    Debug.Log("此AB包已经被删:" + file.Name);
                    if (File.Exists(file.FullName))
                    {
                        File.Delete(file.FullName);
                        File.Delete(file.FullName + ".meta");
                    }
                }
            }
        }

        static bool ContainABName(string name, string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (name == strs[i] || name == strs[i] + ".manifest")
                    return true;
            }
            return false;
        }
        #endregion
    }
}

