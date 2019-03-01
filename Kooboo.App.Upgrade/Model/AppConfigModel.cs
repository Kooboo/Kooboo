//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Kooboo.App.Upgrade.Model
{
    [XmlRoot("configuration")]
    public class AppConfigModel
    {
        [XmlArray("appSettings")]
        [XmlArrayItem("add")]
        public List<AddModel> AppSettings { get; set; }
    }

    public class AddModel
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
