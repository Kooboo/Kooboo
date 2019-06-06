//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Reflection.Emit;


namespace Kooboo.Web.Menus
{
    public static class MenuHelper
    { 
        public static string AdminUrl(string relativeUrl)
        {
            return "/_Admin/" + relativeUrl;
        } 
         

        public static string ApiUrl<TApi>(string methodname) where TApi: Kooboo.Api.IApi
        {
            var modelname = GetApiName(typeof(TApi));
            return ApiUrl(modelname, methodname); 
        } 

        public static string ApiUrl(string modelname, string methodname)
        { 
            return "/_api/" + modelname + "/" + methodname;
        }

        public static string SiteObjectApiUrl<TSiteModel>(string methodname) where TSiteModel : Kooboo.Data.Interface.ISiteObject
        {
            var apiprovider = Web.SystemStart.CurrentApiProvider;

            var modelname = typeof(TSiteModel).Name; 

            foreach (var item in apiprovider.List)
            {
               if (item.Value.ModelName == modelname)
                {
                    return ApiUrl(modelname, methodname); 
                }
            }
            return null; 
        }

        private static string GetApiName(Type ApiType)
        {
            var method = ApiType.GetProperty("ModelName").GetGetMethod();
            var dynamicMethod = new DynamicMethod("meide", typeof(string),
                                                  Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Call, method);
            generator.Emit(OpCodes.Ret);
            var silly = (Func<string>)dynamicMethod.CreateDelegate(
                           typeof(Func<string>));
            return silly();
        } 
         

    }
}
