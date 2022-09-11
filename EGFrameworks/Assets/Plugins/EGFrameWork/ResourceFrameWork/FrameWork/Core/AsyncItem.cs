using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource.Core
{
    public class AsyncItem
    {
        public uint CRC { get; set; }
        public string Path { get; set; }
        public LoadPriority LoadPriority { get; set; } = LoadPriority.Low;
        public bool IsSprite { get; set; }
        public List<AsyncCallBack> CallBackList { get; set; } = new List<AsyncCallBack>();
        public bool Clear { get; set; } = true;
        public void Reset()
        {
            CRC = 0;
            Path = null;
            LoadPriority = LoadPriority.Low;
            CallBackList.Clear();
            IsSprite = false;
            Clear = true;
        }
    }

    public class AsyncCallBack
    {
        public AsyncCompleted OnCompleted { get; set; }
        public object[] ParamList { get; set; }
        public void Reset()
        {
            OnCompleted = null;
            ParamList = null;
        }
    }
}
