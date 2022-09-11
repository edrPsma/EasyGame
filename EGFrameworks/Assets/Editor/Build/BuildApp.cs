using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using EG.Resource.Core;

namespace EG.Resource.Core
{
    public class BuildApp
    {
        public static string mAppName = Application.productName;
        public static string mAndroidPath = Application.dataPath + "/../BuildTarget/Android";
        public static string mIOSPath = Application.dataPath + "/../BuildTarget/IOS";
        public static string mWindowsPath = Application.dataPath + "/../BuildTarget/Windows";
        [MenuItem("EasyGame/3.打标准包", false, 2)]
        public static void Build()
        {
            //打AB包
            ABBuilder.Build();

            BuildAppInfo buildAppInfo = GetPCBuildAppInfo();
            string suffix = SetPCInfo(buildAppInfo);

            //生成可执行文件
            string abPath = Application.dataPath + "/../AssetBundle";
            Copy(abPath, Application.streamingAssetsPath);
            string targetPath = "";

            CreateExportDir();

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                targetPath = mAndroidPath + "/" + mAppName + "_Android" + suffix +
                    EditorUserBuildSettings.activeBuildTarget +
                    string.Format("_{0:yyyy_MM_dd_HH_mm}", System.DateTime.Now) +
                    ".apk";
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                targetPath = mIOSPath + "/" + mAppName + "_IOS" + suffix +
                    string.Format("_{0:yyyy_MM_dd_HH_mm}", System.DateTime.Now);
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
            {
                targetPath = mWindowsPath + "/" + mAppName + "_PC" + suffix +
                    string.Format("_{0:yyyy_MM_dd_HH_mm}/{1}.exe", System.DateTime.Now, mAppName);
            }

            BuildPipeline.BuildPlayer(
                FindEnableEditorScenes(),
                targetPath,
                EditorUserBuildSettings.activeBuildTarget, BuildOptions.None
            );

            DeleteDir(Application.streamingAssetsPath);
        }

        #region 根据Jenkins的参数获取打包设置信息
        static BuildAppInfo GetPCBuildAppInfo()
        {
            string[] param = Environment.GetCommandLineArgs();
            BuildAppInfo info = new BuildAppInfo();
            foreach (string str in param)
            {
                if (str.StartsWith("Version"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        info.Version = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Build"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        info.Build = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Name"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        info.Name = tempParam[1].Trim();
                    }
                }
                else if (str.StartsWith("Debug"))
                {
                    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempParam.Length == 2)
                    {
                        bool.TryParse(tempParam[1].Trim(), out info.Debug);
                    }
                }
            }
            return info;
        }
        static string SetPCInfo(BuildAppInfo info)
        {
            string suffix = "_";
            if (!string.IsNullOrEmpty(info.Version))
            {
                PlayerSettings.bundleVersion = info.Version;
                suffix += info.Version;
            }

            if (!string.IsNullOrEmpty(info.Build))
            {
                PlayerSettings.macOS.buildNumber = info.Build;
                suffix += "_" + info.Build;
            }

            if (!string.IsNullOrEmpty(info.Name))
            {
                PlayerSettings.productName = info.Name;
            }

            if (info.Debug)
            {
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.connectProfiler = true;
                suffix += "_" + info.Debug;
            }
            else
            {
                EditorUserBuildSettings.development = false;
            }
            return suffix;
        }
        #endregion

        #region 获取所有添加的场景
        static string[] FindEnableEditorScenes()
        {
            List<string> editorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;

                editorScenes.Add(scene.path);
            }
            return editorScenes.ToArray();
        }
        #endregion

        #region 拷贝AB包
        static void Copy(string path, string targetPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                //获取目录下（不包含子目录）的文件和子目录
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(targetPath + "/" + i.Name))
                        {
                            //目标目录下不存在此文件夹即创建子文件夹
                            Directory.CreateDirectory(targetPath + "/" + i.Name);
                        }
                        //递归调用复制子文件夹
                        Copy(i.FullName, targetPath + "/" + i.Name);
                    }
                    else
                    {
                        //不是文件夹即复制文件，true表示可以覆盖同名文件
                        File.Copy(i.FullName, targetPath + "/" + i.Name, true);
                    }
                }
            }
            catch
            {
                Debug.LogError("无法复制,form:" + path + ",to:" + targetPath);
            }
        }
        #endregion

        #region 删除文件夹
        static void DeleteDir(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    DirectoryInfo[] childs = dir.GetDirectories();
                    foreach (DirectoryInfo child in childs)
                    {
                        child.Delete(true);
                    }
                    dir.Delete(true);
                }
                if (File.Exists(path + ".meta"))
                {
                    File.Delete(path + ".meta");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        #endregion

        #region 创建导出文件夹
        static void CreateExportDir()
        {
            if (!Directory.Exists(mAndroidPath))
            {
                Directory.CreateDirectory(mAndroidPath);
            }
            if (!Directory.Exists(mIOSPath))
            {
                Directory.CreateDirectory(mIOSPath);
            }
            if (!Directory.Exists(mWindowsPath))
            {
                Directory.CreateDirectory(mWindowsPath);
            }
        }
        #endregion

        #region 创建打包日志
        static void CreateBuildLog(string context)
        {
            FileInfo fileInfo = new FileInfo(Application.dataPath + "/../BuildLog.txt");
            StreamWriter sw = fileInfo.CreateText();
            sw.WriteLine(context);
            sw.Close();
            sw.Dispose();
        }
        #endregion

        #region 打PC包
        public static void BuildPC()
        {
            //打AB包
            ABBuilder.Build();
            BuildAppInfo buildAppInfo = GetPCBuildAppInfo();
            string suffix = SetPCInfo(buildAppInfo);
            //生成可执行文件
            string abPath = Application.dataPath + "/../AssetBundle";
            Copy(abPath, Application.streamingAssetsPath);
            string name = mAppName + "_PC" + suffix +
                string.Format("_{0:yyyyMMddHHmm}/{1}.exe", System.DateTime.Now, mAppName);

            string targetPath = mWindowsPath + "/" + name;

            CreateExportDir();
            CreateBuildLog(name.Replace("/" + mAppName + ".exe", ""));

            BuildPipeline.BuildPlayer(
                FindEnableEditorScenes(),
                targetPath,
                BuildTarget.StandaloneWindows64, BuildOptions.None
            );

            DeleteDir(Application.streamingAssetsPath);
        }
        #endregion
    }
}

