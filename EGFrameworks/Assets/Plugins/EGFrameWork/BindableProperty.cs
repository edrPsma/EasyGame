using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG
{
    /// <summary>
    /// 可绑定属性
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class BindableProperty<T> where T : IEquatable<T>, IComparable<T>
    {
        public BindableProperty() { }
        public BindableProperty(T value, Action<T> onValueChanged = null, Action<T> onBeforeChange = null)
        {
            mValue = value;
            OnValueChanged = onValueChanged;
            OnBeforeChanged = onBeforeChange;
        }

        private T mValue = default(T);

        /// <summary>
        /// 值
        /// </summary>
        public T Value
        {
            get => mValue;
            set
            {
                if (!value.Equals(mValue))
                {
                    OnBeforeChanged?.Invoke(mValue);
                    mValue = value;
                    OnValueChanged?.Invoke(mValue);
                }
            }
        }

        /// <summary>
        /// 变化后要执行的方法
        /// </summary>
        public Action<T> OnValueChanged { get; set; }

        /// <summary>
        /// 变化后要执行的方法
        /// </summary>
        public Action<T> OnBeforeChanged { get; set; }

        public void Synchronization()
        {
            OnValueChanged?.Invoke(Value);
        }
    }
}
