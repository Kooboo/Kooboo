//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;

namespace Kooboo.Api
{
    public class ApiProvider : IApiProvider
    {
        public ApiProvider()
        {
            List = new Dictionary<string, IApi>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IApi)))
            {
                Set(item);
            }
        }

        public Dictionary<string, IApi> List
        {
            get; set;
        }

        public string ApiPrefix { get; set; } = "/_api";
        public Func<ApiCall, ApiMethod> GetMethod { get; set; }
        public Func<RenderContext, ApiMethod, bool> CheckAccess { get; set; }

        public void Set(Type apitype)
        {
            if (Activator.CreateInstance(apitype) is IApi instance) List[instance.ModelName] = instance;
        }

        public IApi Get(string modelName)
        {
            if (!string.IsNullOrEmpty(modelName))
                return List.ContainsKey(modelName) ? List[modelName] : null;

            return null;
        }
    }
}