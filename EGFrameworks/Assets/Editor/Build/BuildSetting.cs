using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EG.Resource.Core
{
    public class BuildSetting : ScriptableWizard
    {
        static BuildingConfig config;
        [Tooltip("打包后的AB包配置文件名")]
        public string ConfigurationFileABPackageName = "abconfig";
        [Tooltip("所有场景路径")]
        public List<string> AllScenePath = new List<string>();
        [Tooltip("所有预制体路径")]
        public List<string> AllPrefabPath = new List<string>();
        [Tooltip("所有文件夹路径")]
        public List<DirectoryABInfo> AllDirectoryPath = new List<DirectoryABInfo>();
        [MenuItem("EasyGame/1.打包AB设置", false, 0)]
        static void Change()
        {
            config = null;

            BuildSetting wizad = ScriptableWizard.DisplayWizard<BuildSetting>("打包设置", "确定");
            wizad.minSize = new Vector2(280, 200);
        }

        private void OnWizardUpdate()
        {
            if (config) return;
            if (!config)
            {
                config = Resources.Load<BuildingConfig>("ABConfig");
            }

            SyncSettings();
        }

        private void OnWizardCreate()
        {
            SerializedObject sobj = new SerializedObject(config);
            sobj.FindProperty("ConfigurationFileABPackageName").stringValue = ConfigurationFileABPackageName;
            sobj.FindProperty("AllScenePath").Dispose();
            for (int i = 0; i < AllScenePath.Count; i++)
            {
                sobj.FindProperty("AllScenePath").GetArrayElementAtIndex(i).stringValue = AllScenePath[i];
            }
            sobj.FindProperty("AllPrefabPath").Dispose();
            for (int i = 0; i < AllPrefabPath.Count; i++)
            {
                sobj.FindProperty("AllPrefabPath").GetArrayElementAtIndex(i).stringValue = AllPrefabPath[i];
            }
            sobj.FindProperty("AllDirectoryPath").Dispose();
            for (int i = 0; i < AllDirectoryPath.Count; i++)
            {
                sobj.FindProperty("AllDirectoryPath").GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue = AllDirectoryPath[i].name;
                sobj.FindProperty("AllDirectoryPath").GetArrayElementAtIndex(i).FindPropertyRelative("path").stringValue = AllDirectoryPath[i].path;
            }
            sobj.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            Debug.Log("修改成功");
            Resources.UnloadAsset(config);
            AllScenePath.Clear();
            AllPrefabPath.Clear();
            AllDirectoryPath.Clear();
        }

        void SyncSettings()
        {
            ConfigurationFileABPackageName = config.ConfigurationFileABPackageName;
            AllScenePath = config.AllScenePath;
            AllPrefabPath = config.AllPrefabPath;
            AllDirectoryPath = config.AllDirectoryPath;
        }
    }
}

