using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Core;
using EG.UI.Core;
using EG.Resource;

namespace EG.UI
{
    public class UIManager : AbstractUtility
    {
        public string CurPanel { get; private set; }
        public Transform MainCanvas => EG.UI.Core.MainCanvas.Instance.transform;
        private Transform mPreLoadArea;
        public Transform PreLoadArea
        {
            get
            {
                if (mPreLoadArea == null)
                {
                    mPreLoadArea = MainCanvas.Find("PreloadArea");
                }
                return mPreLoadArea;
            }
        }
        //保存实例化后的BasePanel
        Dictionary<string, IPanel> mLoadBasePanelDic = new Dictionary<string, IPanel>();
        //页面容器栈
        Stack<IPanel> mBasePanelStack = new Stack<IPanel>();
        //保存预加载的BasePanel
        List<IPanel> mPreLoadBasePanel = new List<IPanel>();

        IPanel GetBasePanel(string path, bool preLoad = false)
        {
            IPanel result = null;
            if (mLoadBasePanelDic.TryGetValue(path, out result))
            {
                if (mPreLoadBasePanel.Contains(result))
                {
                    (result as BasePanel).transform.SetParent(MainCanvas);
                    mPreLoadBasePanel.Remove(result);
                }
                return result;
            }

            GameObject panelObj = ContainerManager.Instance.GetUtility<ObjectPoolManager>().InstantiateObject(path, MainCanvas);
            if (preLoad)
            {
                panelObj.transform.SetParent(PreLoadArea);
            }
            result = panelObj.GetComponent<BasePanel>();
            result?.Init();
            mLoadBasePanelDic.Add(path, result);
            return result;
        }


        public BasePanel PushPanel(string path)
        {
            if (mBasePanelStack.Count > 0)
            {
                IPanel topPanel = mBasePanelStack?.Peek();
                topPanel?.Pause();
            }

            IPanel panel = GetBasePanel(path);
            mBasePanelStack?.Push(panel);
            CurPanel = panel.ToString();
            panel?.Enter();
            return panel as BasePanel;
        }

        public void PopPanel()
        {
            if (mBasePanelStack.Count <= 0) return;

            IPanel topPanel = mBasePanelStack?.Pop();
            topPanel?.Exit();

            if (mBasePanelStack.Count > 0)
            {
                IPanel panel = mBasePanelStack?.Peek();
                CurPanel = panel.ToString();
                panel?.Resume();
            }
        }

        public void Clear()
        {
            mLoadBasePanelDic.Clear();
            mBasePanelStack.Clear();
        }

        public BasePanel PreLoadPanel(string path)
        {
            BasePanel result = GetBasePanel(path, true) as BasePanel;
            mPreLoadBasePanel.Add(result);
            return result;
        }
    }
}
