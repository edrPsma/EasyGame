using EG.Core;

namespace EG
{
    public abstract class AbstractCommand : ICommand
    {
        public void EventTrigger<T>() where T : new()
        {
            EasyEvent.Instance.Trigger<T>();
        }

        public void EventTrigger<T>(T instance) where T : new()
        {
            EasyEvent.Instance.Trigger<T>(instance);
        }


        public void Execute()
        {
            OnExecute();
        }
        protected abstract void OnExecute();

        public T GetSystem<T>() where T : class, ISystem
        {
            return ContainerManager.Instance.GetSystem<T>() as T;
        }

        public object GetSystem(object key)
        {
            return ContainerManager.Instance.GetSystem(key);
        }

        public T GetModel<T>() where T : class, IModel
        {
            return ContainerManager.Instance.GetModel<T>() as T;
        }

        public object GetModel(object key)
        {
            return ContainerManager.Instance.GetModel(key);
        }

        public T GetUtility<T>() where T : class, IUtility
        {
            return ContainerManager.Instance.GetUtility<T>() as T;
        }

        public object GetUtility(object key)
        {
            return ContainerManager.Instance.GetUtility(key);
        }
    }
}
