using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource.Core
{
    public class ABItem
    {
        public AssetBundle AssetBundle { get; set; }
        public int RefCount { get; set; }
        public void Reset()
        {
            AssetBundle = null;
            RefCount = 0;
        }
    }
}
