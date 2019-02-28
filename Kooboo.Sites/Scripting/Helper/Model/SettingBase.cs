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
    public class SettingBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 示例说明
        /// </summary>
        public string Description { get; set; }

    }
}
