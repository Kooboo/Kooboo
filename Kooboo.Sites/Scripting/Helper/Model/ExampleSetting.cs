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
    public class ExampleSetting:SettingBase
    {
        #region example
        /// <summary>
        /// 使用例子
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public string Example { get; set; }

        [XmlText]
        [JsonIgnore]
        [XmlElement("Example")]
        public XmlNode[] CDataExample
        {
            get
            {
                var doc = new XmlDocument();

                if (NeedCData(Example))
                {
                    return new XmlNode[] { doc.CreateCDataSection(Example) };
                }

                return new XmlNode[] { doc.CreateTextNode(Example) };
            }
            set
            {
                if (value == null || value.Length != 1)
                {
                    Example = null;
                    return;
                }
                var node = value[0];
                if (node == null)
                {
                    Example = null;
                    return;
                }
                Example = node.Value;
            }
        }

        private bool NeedCData(string example)
        {
            return Example != null && (
                Example.IndexOf("<", StringComparison.OrdinalIgnoreCase) > -1
                || Example.IndexOf("&", StringComparison.OrdinalIgnoreCase) > -1
                || Example.IndexOf(">", StringComparison.OrdinalIgnoreCase) > -1
                );
        }
        #endregion
    }
}
