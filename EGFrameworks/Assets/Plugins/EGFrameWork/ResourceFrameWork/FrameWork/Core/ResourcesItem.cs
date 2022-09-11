using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.Resource.Core
{
    public class ResourcesItem
    {
        public uint CRC { get; set; }
        public string ABName { get; set; }
        public string AssetName { get; set; }
        public List<string> ABDependce { get; set; }
        public AssetBundle AssetBundle { get; set; }
        public Object Object { get; set; }
        public int RefCount { get; set; }
        public bool Clear { get; set; } = true;
    }
}
