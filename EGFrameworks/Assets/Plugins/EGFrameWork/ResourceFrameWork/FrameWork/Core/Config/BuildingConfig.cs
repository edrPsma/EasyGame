using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource.Core
{
    //[CreateAssetMenu(fileName = "BuildingConfig", menuName = "创建AB包打包配置文件", order = 1)]
    public class BuildingConfig : ScriptableObject
    {
        public string ConfigurationFileABPackageName = "abconfig";
        public List<string> AllScenePath = new List<string>();
        public List<string> AllPrefabPath = new List<string>();
        public List<DirectoryABInfo> AllDirectoryPath = new List<DirectoryABInfo>();
    }

    [System.Serializable]
    public struct DirectoryABInfo
    {
        public string name;
        public string path;
    }
}
