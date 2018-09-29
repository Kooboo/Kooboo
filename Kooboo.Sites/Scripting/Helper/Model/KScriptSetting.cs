//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Kooboo.Sites.Scripting.Helper
{
    public class KScriptSetting: ExampleSetting
    {

        public List<Property> Props { get; set; } = new List<Property>();

        public List<Method> Methods { get; set; } = new List<Method>();

    }
}
