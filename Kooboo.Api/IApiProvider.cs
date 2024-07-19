//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using Kooboo.Data.Context;

namespace Kooboo.Api
{
    public interface IApiProvider
    {
        Dictionary<string, Dictionary<string, IApi>> List { get; }

        //this seems like only for unit test now. 
        public void Set(Type apitype)
        {
            var instance = Activator.CreateInstance(apitype) as IApi;
            AddApi(instance);
        }

        public IApi Get(ApiCommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.ObjectType))
            {
                return null;
            }

            if (!List.ContainsKey(command.ObjectType)) return null;
            var apis = List[command.ObjectType];
            if (apis.ContainsKey(command.Version)) return apis[command.Version];
            return apis.ContainsKey(ApiVersion.V1) ? apis[ApiVersion.V1] : null;
        }

        string ApiPrefix { get; set; }

        Func<ApiCall, ApiMethod> GetMethod { get; set; }

        Func<RenderContext, ApiMethod, bool> CheckAccess { get; set; }

        public void AddApi(IApi api)
        {
            if (api == null) return;

            var version = ApiHelper.GetVersion(api);

            if (!List.ContainsKey(api.ModelName))
            {
                List[api.ModelName] = new Dictionary<string, IApi>();
            }

            List[api.ModelName][version] = api;
        }
    }
}