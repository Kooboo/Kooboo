//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Converter
{
    public class ConvertResult
    {
        public string ConvertToType { get; set; }

        /// <summary>
        /// The name of this site object... 
        /// </summary>
        public string Name { get; set; }

        public string HtmlBody { get; set; }
        public string Data { get; set; }
        public string KoobooId { get; set; }
        public string LayoutName { get; set; }

        public List<ConvertSetting> Settings { get; set; }
    }

    public class ConvertSetting
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

}
