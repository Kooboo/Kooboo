using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Api;
using Kooboo.Model.Setting;
using Kooboo.Model.ValidationRules;
using System.Reflection;
using Kooboo.Model.Render.API;

namespace Kooboo.Model.Render
{
    public class ApiAnalysize
    {
        private Type _koobooModelType;
        public ApiModel ApiModel;

        private static  List<IKApi> apiList { get; set; }

        public static List<IKApi> ApiList
        {
            get
            {
                if (apiList == null)
                {
                    apiList = new List<IKApi>();
                    foreach (var item in Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IKApi)))
                    {
                        var instance = Activator.CreateInstance(item) as IKApi;

                        if (instance != null)
                        {
                            apiList.Add(instance);
                        }
                    }
                }

                return apiList;
            }
        }
        
        public ApiAnalysize(string api,IApiProvider provider)
        {
            var iapi = ApiList.Where(a => a.isMatch(api)).FirstOrDefault();
            if (iapi == null)
                throw new Exception(string.Format("api is wrong"));

            ApiModel = iapi.Get(api,provider);
            
            _koobooModelType = GetKoobooModelType();
        }
        public Dictionary<string, string> GetDataList()
        {
            var dic = new Dictionary<string, string>();
            if (_koobooModelType == null) return dic;

            var props = _koobooModelType.GetProperties();
            var attrPropers = typeof(Attribute).GetProperties();

            if (props != null)
            {
                foreach (var prop in props)
                {
                    //exclude attribute props
                    var isAttrProp = attrPropers.ToList().Exists(a => a.Name == prop.Name);
                    if (isAttrProp) continue;
                    string value = "null";
                    if (prop.PropertyType.IsValueType)
                    {
                        value = Activator.CreateInstance(prop.PropertyType).ToString().ToLower();
                    }

                    dic.Add(prop.Name, value);
                }
            }

            return dic;
        }

        public Dictionary<string, List<ValidationRule>> GetRules()
        {
            var dic = new Dictionary<string, List<ValidationRule>>();
            if (_koobooModelType == null) return dic;
            var props = _koobooModelType.GetProperties();
            if (props != null)
            {
                foreach (var prop in props)
                {
                    var rules = prop.GetCustomAttributes(typeof(ValidationRule));
                    if (rules != null && rules.Count() > 0)
                    {
                        var ValidationRules = rules.ToList().Select(r => r as ValidationRule).ToList();
                        dic.Add(prop.Name, ValidationRules);
                    }
                }
            }

            return dic;
        }

        private Type GetKoobooModelType()
        {
            var methodInfo = ApiModel.GetMethodInfoByApi();
            if (methodInfo == null)
            {
                throw new Exception(string.Format("method {0}. doesn't exist", ApiModel.Obj, ApiModel.Api));
            }
            var type = methodInfo.GetParameters().ToList()
                .Where(p => typeof(IKoobooModel).IsAssignableFrom(p.ParameterType))
                .Select(p=>p.ParameterType).FirstOrDefault() as Type;

            return type;
        }

       

        
    }
}
