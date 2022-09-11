using System.Collections.Generic;
using System.Xml.Serialization;

namespace EG.Resource.Core
{
    [System.Serializable]
    public class ABConfig
    {
        [XmlElement("ABList")]
        public List<ABBase> ABList { get; set; }

    }

    [System.Serializable]
    public class ABBase
    {
        [XmlAttribute("Path")]
        public string Path { get; set; }
        [XmlAttribute("Crc")]
        public uint Crc { get; set; }
        [XmlAttribute("ABName")]
        public string ABName { get; set; }
        [XmlAttribute("AssetName")]
        public string AssetName { get; set; }
        [XmlElement("ABDependce")]
        public List<string> ABDependce { get; set; }
    }
}
