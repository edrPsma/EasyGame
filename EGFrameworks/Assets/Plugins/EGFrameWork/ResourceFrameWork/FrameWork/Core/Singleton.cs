using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource.Core
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T mInstance = null;
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new T();
                }
                return mInstance;
            }
        }
        protected Singleton() { }
    }
}
