using System.Linq;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Api.Methods;
using Kooboo.Sites.OAuth2;
using Kooboo.Sites.OAuth2.Apple;

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
            if (handleType == null) throw new Exception("Handle type not found");
            var response = new PlainResponse();
            var instance = Activator.CreateInstance(handleType, new[] { call.Context }) as IOAuth2;
            var query = new Dictionary<string, object>();

            if (call.Context.Request.QueryString?.AllKeys != default)
            {
                foreach (var item in call.Context.Request.QueryString.AllKeys)
                {
                    query.Add(item, call.Context.Request.QueryString.Get(item));
                }
            }

            //apple oauth login special handle
            if (instance is AppleLogin)
            {
                if (call.Context.Request.Forms != null)
                {
                    foreach (var form in call.Context.Request.Forms)
                    {
                        query.Add(form.ToString(), call.Context.Request.Forms.Get(form.ToString()));
                    }
                }
            }

            response.Content = instance.Callback(query);
            response.ContentType = call.Context.Response.ContentType;
            response.statusCode = call.Context.Response.StatusCode;
            return response;
        }
    }
}
