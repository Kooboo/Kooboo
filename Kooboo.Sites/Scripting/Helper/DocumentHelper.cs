//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Helper
{
    public class DocumentHelper
    {
        public const string RootName = "k";
        public static string LowerCaseFirstChar(string name)
        {
            if (!string.IsNullOrEmpty(name) && char.IsUpper(name[0]))
            {
                name = char.ToLower(name[0]) + name.Substring(1);
            }
            return name;
        }
        public static string GetTypeUrl(string name)
        {
            return "?name=" + name;
        }
        public static string GetMethodUrl(string name, string method, string param)
        {
            return string.Format("?name={0}&method={1}&param={2}", name, method, param);
        }

        public static string GetViewUrl(string name)
        {
            return string.Format("?kview={0}", name);
        }
    }
}
