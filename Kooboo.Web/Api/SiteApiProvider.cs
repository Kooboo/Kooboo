//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Sites.Models;
using Kooboo.Sites.Contents.Models;
using Kooboo.Lib.Reflection;
using Kooboo.Api;
using Kooboo.Data.Context;

namespace Kooboo.Web.Api
{
    public class SiteApiProvider : IApiProvider
    {
        public SiteApiProvider()
        {
            List = DefaultList();
            CheckAccess = Kooboo.Web.Backend.ApiPermission.IsAllow; 
        }

        private Dictionary<string, IApi> DefaultList()
        {
            var defaultlist = new Dictionary<string, IApi>(StringComparer.OrdinalIgnoreCase);

            var alldefinedTypes = GetAllDefinedApi();
            foreach (var item in alldefinedTypes)
            {
                var instance = Activator.CreateInstance(item) as IApi;
                AddApi(defaultlist, instance);
            }

            AddApi(defaultlist, new SiteObjectApi<CmsFile>());
            AddApi(defaultlist, new SiteObjectApi<CmsCssRule>());

            AddApi(defaultlist, new SiteObjectApi<Folder>());
            AddApi(defaultlist, new SiteObjectApi<ViewDataMethod>());

            return defaultlist;
        }


        private object _locker = new object();

        public Dictionary<string, IApi> List
        {
            get; set;
        }

        public string ApiPrefix { get; set; } = "/_api";
        public Func<ApiCall, ApiMethod> GetMethod {
            get
            {
                return getmethod; 
            }
            set
            {

            }
        }

        public Func<RenderContext, ApiMethod, bool> CheckAccess { get; set; }

        public ApiMethod  getmethod(ApiCall call)
        {
            return Kooboo.Module.ModuleApiHelper.GetApiMethod(call);  
        }

        internal void AddApi(Dictionary<string, IApi> currentlist, IApi instance)
        {
            if (instance != null && currentlist != null)
            {
                var name = instance.ModelName;
                currentlist[name] = instance;     
            }
        }

        private List<Type> GetAllDefinedApi()
        {
            return AssemblyLoader.LoadTypeByInterface(typeof(IApi));
        }

        public void Set(Type apitype)
        {
            var instance = Activator.CreateInstance(apitype) as IApi;
            AddApi(List, instance);
        }

        public IApi Get(string ModelName)
        {
            if (string.IsNullOrEmpty(ModelName))
            {
                return null;
            }
            if (List.ContainsKey(ModelName))
            {
                return List[ModelName];
            }
            return null; 
        }
    }
}
