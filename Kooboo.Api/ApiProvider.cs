//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using Kooboo.Data.Context;

namespace Kooboo.Api
{
    public class ApiProvider : IApiProvider
    {
        public ApiProvider()
        {
            IApiProvider me = this;
            foreach (var item in Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IApi)))
            {
                me.Set(item);
            }
        }

        public ApiProvider(List<Type> ApiTypes)
        {
            IApiProvider me = this;
            foreach (var item in ApiTypes)
            {
                me.Set(item);
            }
        }

        public Dictionary<string, Dictionary<string, IApi>> List { get; } = new(StringComparer.OrdinalIgnoreCase);

        public string ApiPrefix { get; set; } = "/_api";
        public Func<ApiCall, ApiMethod> GetMethod { get; set; }
        public Func<RenderContext, ApiMethod, bool> CheckAccess { get; set; }
    }
}