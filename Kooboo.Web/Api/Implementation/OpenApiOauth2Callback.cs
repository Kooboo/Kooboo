using System.Linq;
using System.Text;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Api.Methods;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.OpenApi;

namespace Kooboo.Web.Api.Implementation
{
    public class OpenApiOauth2Callback : IApi, IDynamicApi
    {
        public string ModelName => "openapioauth2callback";

        public bool RequireSite => false;

        public bool RequireUser => false;

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
            var code = call.Context.Request.QueryString.Get("code");
            var siteId = Guid.Parse(call.Command.Method);
            var authorizeId = Guid.Parse(call.Command.Parameters.First());
            var name = call.Command.Parameters.Last();
            var site = Data.Config.AppHost.SiteRepo.Get(siteId);
            var authorize = site.SiteDb().OpenApiAuthorize.Get(authorizeId);
            Sites.Models.OpenApiAuthorize.Security data = null;
            authorize?.Securities?.TryGetValue(name, out data);
            var sender = HttpSender.GetSender(Sites.OpenApi.Securities.OAuth2.ContentType);

            var body = new Dictionary<string, object> {
                {"grant_type","authorization_code" },
                {"code",code },
                {"client_id",data.ClientId },
                {"client_secret",data.ClientSecret },
                {"redirect_uri",data.RedirectUrl }
            };

            var token = Helpers.BasicAuthEncode(data.ClientId, data.ClientSecret);
            Sites.OpenApi.Cache.Get(site).Documents.TryGetValue(authorize.OpenApiName, out var doc);
            var flow = doc.GetOAuthFlow(name);

            var result = sender.Send(flow.TokenUrl.ToString(), "POST", body, new Dictionary<string, string>
            {
                { "Authorization", $"Basic {token}"}
            }, null);

            var dic = result as IDictionary<string, object>;
            if (dic.ContainsKey("hasError")) return new JsonResponse(result);

            if (dic.TryGetValue("access_token", out var access_token))
            {
                data.AccessToken = access_token.ToString();
            }

            if (dic.TryGetValue("refresh_token", out var refresh_token))
            {
                data.RefreshToken = refresh_token.ToString();
            }

            if (dic.TryGetValue("expires_in", out var expires_in))
            {
                data.ExpiresIn = DateTime.UtcNow.AddSeconds(Convert.ToInt32(expires_in));
            }

            site.SiteDb().OpenApiAuthorize.AddOrUpdate(authorize);
            Sites.OpenApi.Cache.Remove(site);

            return new BinaryResponse
            {
                ContentType = "text/html",
                BinaryBytes = Encoding.UTF8.GetBytes(@"<script>window.close();</script>")
            };
        }
    }
}
