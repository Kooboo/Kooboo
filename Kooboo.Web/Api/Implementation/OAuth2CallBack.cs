using Jint.Native.Function;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Api.Methods;
using Kooboo.Data.Interface;
using Kooboo.Sites.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Kooboo.Web.Api.Implementation
{
    public class OAuth2CallBackApi : IApi, IDynamicApi
    {
        public string ModelName => "oauth2callback";

        public bool RequireSite => true;

        public bool RequireUser => false;

        public Lazy<List<Type>> OAuth2Handles { get; set; } = new Lazy<List<Type>>(() => Lib.IOC.Service.GetImplementationTypes(typeof(IOAuth2)), true);

        public DynamicApi GetMethod(string name)
        {
            return new DynamicApi
            {
                Type = GetType(),
                Method = GetType().GetMethod(nameof(WrapMethod))
            };
        }

        public IResponse WrapMethod(ApiCall call)
        {
            var name = call.Command.Method;
            var handleType = OAuth2Handles.Value.FirstOrDefault(f => f.Name == name);
            if (handleType == null) throw new Exception("Can't find handle type");
            var response = new PlainResponse();
            var instance = Activator.CreateInstance(handleType, new[] { call.Context }) as IOAuth2;
            var query = new Dictionary<string, object>();

            foreach (var item in call.Context.Request.QueryString.AllKeys)
            {
                query.Add(item, call.Context.Request.QueryString.Get(item));
            }

            response.Content = instance.Callback(query);
            response.ContentType = call.Context.Response.ContentType;
            response.statusCode = call.Context.Response.StatusCode;
            return response;
        }
    }
}
