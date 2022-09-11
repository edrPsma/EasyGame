using EG.Core;

namespace EG
{
    public interface IView
    {
        void SendCommand<T>() where T : ICommand, new();
        void SendCommand<T>(T instance) where T : ICommand, new();
        IUnRegister EventRegister<T>(System.Action<T> onEvent, bool UnRegisterWhenGameObjectDestory = true) where T : new();
        void EventUnRegister<T>(System.Action<T> onEvent);

        /// <summary>
        /// 获取系统层对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetSystem<T>() where T : class, ISystem;
        /// <summary>
        /// 获取系统层对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetSystem(object key);
        /// <summary>
        /// 获取数据层对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetModel<T>() where T : class, IModel;
        /// <summary>
        /// 获取数据层对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetModel(object key);
        /// <summary>
        /// 获取工具层对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetUtility<T>() where T : class, IUtility;
        /// <summary>
        /// 获取工具层对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetUtility(object key);
    }
}
