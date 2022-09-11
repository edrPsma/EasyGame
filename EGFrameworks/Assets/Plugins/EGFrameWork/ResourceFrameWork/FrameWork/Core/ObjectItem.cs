using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource.Core
{
    public class ObjectItem
    {
        public uint CRC { get; set; }
        public int GUID { get; set; }
        public GameObject GameObject { get; set; }
        public bool Clear { get; set; } = true;
        public bool IsInPool { get; set; }

        public void Reset()
        {
            CRC = 0;
            GUID = 0;
            GameObject = null;
            Clear = true;
            IsInPool = false;
        }
    }
}
