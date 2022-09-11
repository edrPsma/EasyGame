using System.Collections;
using UnityEngine;
using EG.Resource;
using EG.Scene.Core;
using System;
using EG.Resource.Core;
using System.IO;
using EG;
using EG.UI;
using EG.Core;

namespace EG.Scene
{
    public class SceneManager : AbstractUtility
    {
        public BaseSceneState CurScene { get; private set; }
        //切换场景进度条
        public int LoadingProgress { get; private set; } = 0;
        //场景是否加载完
        public bool IsLoading { get; private set; } = false;
        private bool LoadFormAB = true;
        private AsyncOperation asyncOperation;

        protected override void OnInit()
        {
            if (EGrameWork.Model == StartModel.Editor)
            {
                LoadFormAB = false;
            }
        }

        public void ShowScene()
        {
            asyncOperation.allowSceneActivation = true;
        }

        public void LoadScene<T>(bool showInLoadOver = true) where T : BaseSceneState, new()
        {
            BaseSceneState instance = new T();
            LoadScene(instance, showInLoadOver);
        }

        public void LoadScene(BaseSceneState instance, bool showInLoadOver = true)
        {
            if (instance == null) return;

            IsLoading = true;
            (CurScene as IBaseSceneState)?.ExitScene();

            if (LoadFormAB)
            {
                if (CurScene != null)
                {
                    uint crc = CRC32.GetCRC32(CurScene.ScenePath);
                    ResourcesItem item = ABManager.Instance.FindResourceItem(crc);
                    ABManager.Instance.UnLoadResourceAb(item);//卸载场景AB包
                }
            }

            ClearCache();//跳场景清空缓存
            CurScene = instance;
            (CurScene as IBaseSceneState).LoadStart();
            StartCoroutine(LoadSceneAsync(CurScene.ScenePath, showInLoadOver));
        }

        // 跳场景清空缓存
        void ClearCache()
        {
            ContainerManager.Instance.GetUtility<UIManager>().Clear();
            ContainerManager.Instance.GetUtility<ObjectPoolManager>().ClearCache();
            ContainerManager.Instance.GetUtility<ResourcesManager>().ClearCache();
        }

        IEnumerator LoadSceneAsync(string path, bool showInLoadOver)
        {
            string sceneName;
            //如果从AB包中加载
            if (LoadFormAB)
            {
                uint crc = CRC32.GetCRC32(path);
                ResourcesItem item = ABManager.Instance.LoadResourceAB(crc);
                sceneName = item.AssetName.Replace(".unity", "");
            }
            else//在编辑器下加载
            {
                if (!File.Exists(path))
                {
                    Debug.LogError("不存在该场景,请检查:" + path);
                    yield break;
                }

                var strs = path.Split('/');
                sceneName = strs[strs.Length - 1].Replace(".unity", "");
            }

            LoadingProgress = 0;
            int targetProgress = 0;

            asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (asyncOperation != null && !asyncOperation.isDone)
            {
                asyncOperation.allowSceneActivation = false;
                while (asyncOperation.progress < 0.9f)
                {
                    targetProgress = (int)(asyncOperation.progress * 100);
                    yield return new WaitForEndOfFrame();

                    //平滑过渡
                    while (LoadingProgress < targetProgress)
                    {
                        ++LoadingProgress;
                        yield return new WaitForEndOfFrame();
                    }
                }
                //自行加载剩余10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress - 2)
                {
                    ++LoadingProgress;
                    yield return new WaitForEndOfFrame();
                }
                LoadingProgress = 100;
                asyncOperation.allowSceneActivation = showInLoadOver;
                IsLoading = false;
                (CurScene as IBaseSceneState).EnterScene();
            }
        }
    }
}
