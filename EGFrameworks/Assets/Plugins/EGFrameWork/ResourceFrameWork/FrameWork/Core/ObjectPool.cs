using System.Collections.Generic;

namespace EG.Resource.Core
{
    public class ObjectPool<T> where T : class, new()
    {
        protected Stack<T> mPool = new Stack<T>();
        protected int maxCount = 0;
        protected int noRecycleCount = 0;
        public ObjectPool(int maxCount)
        {
            this.maxCount = maxCount;
            for (int i = 0; i < maxCount; i++)
            {
                mPool.Push(new T());
            }
        }

        /// <summary>
        /// 取出
        /// </summary>
        /// <param name="createIfPoolEmpty">对象池为空时是否创建新实例</param>
        /// <returns></returns>
        public T Spawn(bool createIfPoolEmpty)
        {
            T result = null;
            if (mPool.Count > 0)
            {
                result = mPool.Pop();
            }
            else
            {
                if (createIfPoolEmpty)
                {
                    result = new T();
                }
            }

            if (result != null) noRecycleCount++;
            return result;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="obj">实例</param>
        /// <returns>是否回收成功</returns>
        public bool Recycle(T obj)
        {
            if (obj == null) return false;

            noRecycleCount--;

            if (mPool.Count >= maxCount && maxCount > 0)
            {
                obj = null;
                return false;
            }

            mPool.Push(obj);
            return true;
        }
    }
}
