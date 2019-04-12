using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kooboo.Web.Api
{
    public static class CmsApiHelper
    {

        private static Dictionary<string, List<Parameter>> cache = new Dictionary<string, List<Parameter>>(StringComparer.OrdinalIgnoreCase);


        public static List<Kooboo.Api.Parameter> GetParameters(string ObjectType, string Method)
        {
            var apiProvider = GetApiProvider();
            return GetParameters(ObjectType, Method, apiProvider);
        }

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


        public static List<Kooboo.Api.Parameter> GetParameters(Type type, string method)
        {
            var methodinfo = Lib.Reflection.TypeHelper.GetMethodInfo(type, method);
            if (methodinfo != null)
            {
                var para = Kooboo.Api.ApiHelper.GetParameters(methodinfo);
                if (para != null && para.Any())
                {
                    para.RemoveAll(o => o.ClrType == typeof(ApiCall));
                }
                return para;
            } 

            return null;
        }


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
