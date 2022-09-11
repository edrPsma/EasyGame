namespace EG.Core
{
    public interface IModel : IContainable
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
        /// 获取工具层对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetUtility<T>() where T : class, IUtility;

        /// <summary>
        /// 获取工具层对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetUtility(object key);
    }
}
