namespace EG.Core
{
    public interface ICommand
    {
        void Execute();

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        void EventTrigger<T>() where T : new();
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="instance">事件对象</param>
        void EventTrigger<T>(T instance) where T : new();

        /// <summary>
        /// 获取系统对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetSystem<T>() where T : class, ISystem;

        /// <summary>
        /// 获取系统对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetSystem(object key);

        /// <summary>
        /// 货物数据层对象
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
