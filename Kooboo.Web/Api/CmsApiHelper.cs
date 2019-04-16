using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Attributes;
using Kooboo.Api.Methods;

namespace Kooboo.Web.Api
{
    public static class CmsApiHelper
    {

        private static Dictionary<string, List<Parameter>> cache = new Dictionary<string, List<Parameter>>(StringComparer.OrdinalIgnoreCase);

        [Obsolete]
        public static List<Kooboo.Api.Parameter> GetParameters(string ObjectType, string Method)
        {
            var apiProvider = GetApiProvider();
            return GetParameters(ObjectType, Method, apiProvider);
        }


        public static ApiMethod GetApiMethod(string ObjectType, string Method)
        {
            var apiProvider = GetApiProvider();

            var apiobject = apiProvider.Get(ObjectType);

            if (apiobject != null)
            {
                var apimethod = ApiMethodManager.Get(apiobject, Method);

                if (apimethod != null)
                {
                    apimethod.Parameters.RemoveAll(o => o.ClrType == typeof(ApiCall));
                }
                return apimethod;
            }
            return null;
        }


        [Obsolete]
        public static List<Kooboo.Api.Parameter> GetParameters(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                return null;
            }
            var sep = "/\\_".ToCharArray();
            var parts = route.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return GetParameters(parts[0], parts[1]);
            }
            else
            {
                return null;
            }

        }

        [Obsolete]
        public static List<Kooboo.Api.Parameter> GetParameters(string ObjectType, string Method, IApiProvider ApiProvider = null)
        {
            string cachekey = ObjectType + Method;
            if (cache.ContainsKey(cachekey))
            {
                return cache[cachekey];
            }

            if (ApiProvider == null)
            {
                ApiProvider = GetApiProvider();
            }
            if (ApiProvider == null)
            {
                return null;
            }

            var api = ApiProvider.Get(ObjectType);
            if (api != null)
            {
                var para = GetParameters(api.GetType(), Method);

                cache[cachekey] = para;
                return para;
            }
            return null;
        }

        [Obsolete]
        public static List<Kooboo.Api.Parameter> GetParameters(Type type, string method)
        {
            var methodinfo = Lib.Reflection.TypeHelper.GetMethodInfo(type, method);
            if (methodinfo != null)
            {
                var result = new List<Parameter>();

                var para = Kooboo.Api.ApiHelper.GetParameters(methodinfo);
                if (para != null && para.Any())
                {
                    para.RemoveAll(o => o.ClrType == typeof(ApiCall));
                    result.AddRange(para);
                }

                var attrPara = AttributeHelper.GetOptionalParameters(methodinfo);
                if (attrPara != null)
                {
                    foreach (var item in attrPara)
                    {
                        result.Add(new Parameter() { Name = item.name, ClrType = item.clrType });
                    }
                }
                return result;
            }

            return null;
        }

        [Obsolete]
        // Get the default API provider. 
        public static IApiProvider GetApiProvider()
        {
            foreach (var item in Kooboo.Web.SystemStart.Middleware)
            {
                if (item is ApiMiddleware)
                {
                    var api = item as ApiMiddleware;
                    return api.ApiProvider;
                }
            }
            return null;
        }

    }
}
