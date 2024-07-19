//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using Kooboo.Api;
using Kooboo.Data.Context;
using Kooboo.Lib.Reflection;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api
{
    public class SiteApiProvider : IApiProvider
    {
        public SiteApiProvider()
        {
            IApiProvider me = this;
            var types = GetAllDefinedApi();
            foreach (var item in types)
            {
                var instance = Activator.CreateInstance(item) as IApi;
                me.AddApi(instance);
            }

            me.AddApi(new SiteObjectApi<CmsFile>());
            me.AddApi(new SiteObjectApi<CmsCssRule>());
            me.AddApi(new SiteObjectApi<Folder>());
            me.AddApi(new SiteObjectApi<ViewDataMethod>());
            CheckAccess = PermissionService.IsAllow;
        }

        private object _locker = new object();

        public Dictionary<string, Dictionary<string, IApi>> List { get; } = new(StringComparer.OrdinalIgnoreCase);

        public string ApiPrefix { get; set; } = "/_api";

        public Func<ApiCall, ApiMethod> GetMethod { get; set; } = call => Module.ModuleApiHelper.GetApiMethod(call);

        public Func<RenderContext, ApiMethod, bool> CheckAccess { get; set; }

        public static List<Type> GetAllDefinedApi()
        {
            return AssemblyLoader.LoadTypeByInterface(typeof(IApi));
        }
    }
}