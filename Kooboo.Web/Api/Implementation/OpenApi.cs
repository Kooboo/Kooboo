using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Definition;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.OpenApi;

namespace Kooboo.Web.Api.Implementation
{
    public class OpenApi : SiteObjectApi<Sites.Models.OpenApi>
    {
        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            var model = JsonHelper.Deserialize<Sites.Models.OpenApi>(call.Context.Request.Body);
            var result = new OpenApiServices(call.Context).Save(model);
            Sites.OpenApi.Cache.Remove(call.WebSite);
            return result;
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.EDIT)]
        public void UpdateDoc(Guid id, ApiCall call)
        {
            new OpenApiServices(call.Context).UpdateDoc(id);
            Sites.OpenApi.Cache.Remove(call.WebSite);
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            string json = call.GetValue("ids");

            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            var ids = JsonHelper.Deserialize<Guid[]>(json);

            new OpenApiServices(call.Context).Deletes(ids);

            Sites.OpenApi.Cache.Remove(call.WebSite);
            return true;
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<OpenApiAuthorize> GetAuthorizes(Guid id, ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var openApi = siteDb.OpenApi.Get(id);
            return siteDb
                .OpenApiAuthorize
                .Query
                .Where(w => w.OpenApiName == openApi.Name)
                .SelectAll()
                .OrderByDescending(it => it.LastModified)
                .Take(9999);
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.VIEW)]
        public OpenApiAuthorize GetAuthorize(Guid id, ApiCall apiCall)
        {
            return apiCall.WebSite.SiteDb().OpenApiAuthorize.Get(id);
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.EDIT)]
        public void DeleteAuthorizes(ApiCall apiCall)
        {
            var ids = JsonHelper.Deserialize<Guid[]>(apiCall.Context.Request.Body);

            foreach (var item in ids)
            {
                apiCall.WebSite.SiteDb().OpenApiAuthorize.Delete(item, apiCall.Context.User?.Id ?? BuiltInUser.System.Id);
            }

            Sites.OpenApi.Cache.Remove(apiCall.WebSite);
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.EDIT)]
        public void PostAuthorize(ApiCall call)
        {
            var model = JsonHelper.Deserialize<OpenApiAuthorize>(call.Context.Request.Body);
            if (!Regex.IsMatch(model.AuthorizeName, "^[a-zA-Z_]{1}[a-zA-Z0-9_]{0,25}$")) throw new Exception("invalid name");
            call.WebSite.SiteDb().OpenApiAuthorize.AddOrUpdate(model, call.Context.User?.Id ?? BuiltInUser.System.Id);
            Sites.OpenApi.Cache.Remove(call.WebSite);
        }

        public IResponse SaveToken(ApiCall call)
        {
            var id = call.Context.Request.GetValue("id");
            var token = call.Context.Request.GetValue("token");
            var openApiAuthorizeRepository = call.Context.WebSite.SiteDb().OpenApiAuthorize;
            var openApiAuthorize = openApiAuthorizeRepository.GetByNameOrId(id);

            foreach (var item in openApiAuthorize.Securities)
            {
                item.Value.AccessToken = token;
            }

            openApiAuthorizeRepository.AddOrUpdate(openApiAuthorize, call.Context.User?.Id ?? BuiltInUser.System.Id);

            Sites.OpenApi.Cache.Remove(call.WebSite);

            return new BinaryResponse
            {
                ContentType = "text/html",
                BinaryBytes = Encoding.UTF8.GetBytes(@"<script>window.close();</script>")
            };
        }

        public bool IsUniqueAuthorizeName(ApiCall call)
        {
            string name = call.NameOrId;
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(name))
            {
                var repo = call.WebSite.SiteDb().OpenApiAuthorize;

                var value = repo.GetByNameOrId(name);
                if (value != null)
                {
                    return false;
                }
            }

            return true;
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            var result = base.Get(call);

            if (result is Sites.Models.OpenApi openApi)
            {
                SetCodeSource(openApi, call);
            }

            return result;
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var list = base.List(call);

            return list
                .Cast<Sites.Models.OpenApi>()
                .Select(item =>
                {
                    SetCodeSource(item, call);
                    return item;
                })
                .OrderByDescending(it => it.LastModified)
                .ToList<object>();
        }

        [Permission(Feature.OPENAPI, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        void SetCodeSource(Sites.Models.OpenApi openApi, ApiCall call)
        {
            if (openApi.Code == null && openApi.CustomAuthorization != null)
            {
                var code = call.Context.WebSite.SiteDb().Code.GetByNameOrId(openApi.CustomAuthorization);

                if (code != null)
                {
                    openApi.Code = code.Body;
                    openApi.UseCustomCode = true;
                }
            }
        }
    }
}
