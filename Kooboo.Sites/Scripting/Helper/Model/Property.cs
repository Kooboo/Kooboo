//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Kooboo.Sites.Scripting.Helper
{
    public class Property:SettingBase
    {
        public string Type { get; set; }

        [XmlArrayItem("Child")]
        public List<string> Childrens { get; set; }
    }
}
