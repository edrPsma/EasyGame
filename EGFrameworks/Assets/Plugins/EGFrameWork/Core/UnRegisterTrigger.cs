using System.Collections.Generic;
using UnityEngine;

namespace EG.Core
{
    /// <summary>
    /// 注销事件触发器
    /// </summary>
    public class UnRegisterTrigger : MonoBehaviour
    {
        HashSet<IUnRegister> mUnregisters = new HashSet<IUnRegister>();

        /// <summary>
        /// 添加事件注销器
        /// </summary>
        /// <param name="unRegister">事件注销器</param>
        public void AddUnRegister(IUnRegister unRegister)
        {
            mUnregisters.Add(unRegister);
        }

        private void OnDestroy()
        {
            foreach (var item in mUnregisters)
            {
                item.UnRegister();
            }

            mUnregisters.Clear();
        }
    }

    /// <summary>
    /// 事件注销器扩展类
    /// </summary>
    public static class UnRegisterExtra
    {
        /// <summary>
        /// 在游戏物体销毁时注销事件
        /// </summary>
        /// <param name="unRegister">事件注销器</param>
        /// <param name="gameObject">游戏物体</param>
        public static void UnRegisterWhenGameObjectDestory(this IUnRegister unRegister, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterTrigger>();
            }

            trigger.AddUnRegister(unRegister);
        }
    }
}
