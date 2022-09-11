using System;
using System.Collections.Generic;

namespace EG.Core
{
    public class EasyEvent : IEvent
    {
        private EasyEvent() { }
        static EasyEvent mInstance;
        public static EasyEvent Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new EasyEvent();
                }
                return mInstance;
            }
        }
        interface IRegisterations { }

        class Registerations<T> : IRegisterations
        {
            public Action<T> onEvent = _ => { };
        }

        Dictionary<Type, IRegisterations> mEventDir = new Dictionary<Type, IRegisterations>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="onEvent">事件触发后回调方法</param>
        /// <returns>事件注销器接口</returns>
        public IUnRegister Register<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegisterations registerations;

            if (mEventDir.TryGetValue(type, out registerations))
            {

            }
            else
            {
                registerations = new Registerations<T>();
                mEventDir.Add(type, registerations);
            }

            (registerations as Registerations<T>).onEvent += onEvent;

            return new UnRegister<T>() { onEvent = onEvent, Event = this };
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        public void Trigger<T>() where T : new()
        {
            var e = new T();
            Trigger<T>(e);
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="e">事件实例</param>
        public void Trigger<T>(T e)
        {
            var type = typeof(T);
            IRegisterations registerations;

            if (mEventDir.TryGetValue(type, out registerations))
            {
                (registerations as Registerations<T>).onEvent(e);
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="onEvent">事件触发后的回调方法</param>
        public void UnRegister<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegisterations registerations;

            if (mEventDir.TryGetValue(type, out registerations))
            {
                (registerations as Registerations<T>).onEvent -= onEvent;
            }
        }
    }
}
