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
    public class Method: ExampleSetting
    {
        public List<Param> Params = new List<Param>();
        
        public string ReturnType { get; set; }

        /// <summary>
        /// for render
        /// </summary>
        /// <param name="name"></param>
        /// <param name="methodParams"></param>
        /// <returns></returns>
        public bool IsSameMethod(string name, string methodParams)
        {
            if (name.ToLower()!=Name.ToLower()) return false;

            StringBuilder builder = new StringBuilder();
            foreach (var pars in Params)
            {
                builder.Append(pars.Name);
            }
            var str = builder.ToString();
            if (methodParams == null && string.IsNullOrEmpty(str)) return true;
            return builder.ToString() == methodParams;
        }
    }
}
