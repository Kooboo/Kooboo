using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Kooboo.Data
{
    [XmlRoot("kscriptconfig")]
    public class KscriptConfig
    {
        [XmlElement("extensiondlls")]
        public string ExtensionDlls { get; set; }

        [XmlArray("kscripts")]
        [XmlArrayItem("kscript")]
        public List<KScriptItem> Kscripts { get; set; }

        [XmlElement("kcontext")]
        public string KscriptContext { get; set; }

        [XmlElement("setting")]
        public string KscriptSetting { get; set; }

    }

    public class KScriptItem
    {
        [XmlAttribute("namespace")]
        public string NameSpace { get; set;}
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

}
