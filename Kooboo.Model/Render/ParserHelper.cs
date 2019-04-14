using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Kooboo.Dom;

namespace Kooboo.Model.Render
{
    public class ParserHelper
    {
        public static string GetModelNameFromUrl(string url)
        {
            return url.Replace("/", "_");
        }

        public static bool IsJsArray(Type type)
        {
            if (type.IsValueType)
                return false;

            if (type == typeof(String))
                return false;

            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static string GetDefaultValueFromType(Type type)
        {
            if (IsJsArray(type))
                return "[]";

            return "null";
        }

        public static string GetJsonFromModel(ApiMeta.ModelInfo model)
        {
            if (IsJsArray(model.Type))
                return "[]";

            return "{ " + String.Join(", ", model.Properties.Select(o => String.Format($"{ToJsName(o.Name)}: {ParserHelper.GetDefaultValueFromType(o.Type)}"))) + " }";
        }

        public static string ToJsName(string name)
        {
            if (name.Length == 1)
                return name.ToLower();

            return Char.ToLower(name[0]) + name.Substring(1);
        }

        public static string GenerateUrlFromApiParameters(string url, IEnumerable<ApiMeta.PropertyInfo> parameters)
        {
            if (parameters == null || !parameters.Any())
                return url;

            var query = String.Join("&", parameters.Select(o => ToJsName(o.Name)).Select(o => $"{o}={{{o}}}"));
            return $"{url}?{query}";
        }

        public static string ToHtml(IEnumerable<Node> nodes)
        {
            return new OutputHtml.OutputHtml(nodes).ToString();
        }
    }
}
