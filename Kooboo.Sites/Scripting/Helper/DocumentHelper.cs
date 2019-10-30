//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

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
            return $"?name={name}&method={method}&param={param}";
        }

        public static string GetViewUrl(string name)
        {
            return $"?kview={name}";
        }
    }
}