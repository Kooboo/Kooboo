using Kooboo.Api;
using Kooboo.Api.Methods;
using Kooboo.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api
{
    public static class CmsApiHelper
    {
        private static Dictionary<string, List<Parameter>> cache = new Dictionary<string, List<Parameter>>(StringComparer.OrdinalIgnoreCase);

        [Obsolete]
        public static List<Kooboo.Api.Parameter> GetParameters(string objectType, string method)
        {
            var apiProvider = GetApiProvider();
            return GetParameters(objectType, method, apiProvider);
        }

        public static ApiMethod GetApiMethod(string objectType, string method)
        {
            var apiProvider = GetApiProvider();

            var apiobject = apiProvider.Get(objectType);

            if (apiobject != null)
            {
                var apimethod = ApiMethodManager.Get(apiobject, method);

                apimethod?.Parameters.RemoveAll(o => o.ClrType == typeof(ApiCall));
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
            return parts.Length >= 2 ? GetParameters(parts[0], parts[1]) : null;
        }

        [Obsolete]
        public static List<Kooboo.Api.Parameter> GetParameters(string objectType, string method, IApiProvider apiProvider = null)
        {
            string cachekey = objectType + method;
            if (cache.ContainsKey(cachekey))
            {
                return cache[cachekey];
            }

            if (apiProvider == null)
            {
                apiProvider = GetApiProvider();
            }

            var api = apiProvider?.Get(objectType);
            if (api != null)
            {
                var para = GetParameters(api.GetType(), method);

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
                if (item is ApiMiddleware api)
                {
                    return api.ApiProvider;
                }
            }
            return null;
        }
    }
}