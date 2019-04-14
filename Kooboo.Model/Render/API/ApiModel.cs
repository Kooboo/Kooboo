using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Kooboo.Api;

namespace Kooboo.Model.Render.API
{
    public class ApiModel
    {

        public string Obj { get; set; }//obj/class

        public string Api { get; set; }

        public string Method { get; set; } = "get";//get,post,upload

        public IApiProvider Provider;

        public MethodInfo GetMethodInfoByApi()
        {
            var apiobject = Provider.Get(this.Obj);
            if (apiobject == null)
            {
                Console.WriteLine(string.Format("Object type {0} Not Found", this.Obj));
                return null;
            }
            var methodInfo = GetMethodInfo(apiobject.GetType(), this.Api);
            if (methodInfo == null)
            {
                Console.WriteLine(string.Format("Api method {0} Not Found", this.Api));
                return null;
            }
            return methodInfo;
        }

        public string GetApi()
        {
            return string.Format("Kooboo.{0}.{1}", Obj, Api);
        }

        private MethodInfo GetMethodInfo(Type type, string methodname)
        {
            var method = type.GetMethod(methodname);
            if (method != null)
            {
                return method;
            }

            var lowername = methodname.ToLower();

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(type);
            foreach (var item in allmethods)
            {
                if (item.Name.ToLower() == lowername)
                {
                    return item;
                }
            }

            return null;
        }
    }

}
