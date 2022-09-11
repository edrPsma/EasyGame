using System.Collections;
using System.Collections.Generic;
using EG.UI.Core;
using UnityEngine;

namespace EG.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanel : ViewController, IPanel
    {
        #region 接口实现
        public void Init()
        {
            OnInit();
        }
        void IPanel.Enter()
        {
            OnEnter();
        }

        void IPanel.Exit()
        {
            OnExit();
        }

        void IPanel.Pause()
        {
            OnPause();
        }

        void IPanel.Resume()
        {
            OnResume();
        }
        #endregion

        #region 子类重写
        protected virtual void OnInit() { }
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }

        #endregion
    }
}
