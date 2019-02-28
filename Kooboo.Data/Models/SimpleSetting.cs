//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
   public class SimpleSetting
    {
        public string Name { get; set; }

        public string ToolTip { get; set; }
         
        [JsonConverter(typeof(StringEnumConverter))]
        public ControlType ControlType { get; set; } = ControlType.TextBox; 

        public object DefaultValue { get; set; }

        public Type DataType { get; set; } = typeof(string); 

        public Dictionary<string, string> SelectionValues { get; set; } = new Dictionary<string, string>(); 
    }
}
