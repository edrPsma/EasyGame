namespace EG.Core
{
    public interface ISystem : IContainable
    {
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        void EventTrigger<T>() where T : new();
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="instance">事件实例</param>
        void EventTrigger<T>(T instance) where T : new();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="onEvent">事件触发后的回调方法</param>
        /// <returns>事件注销器接口</returns>
        IUnRegister EventRegister<T>(System.Action<T> onEvent) where T : new();

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="onEvent">事件触发后的回调方法</param>
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
