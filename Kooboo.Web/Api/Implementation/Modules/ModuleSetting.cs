using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Modules
{
    public class ModuleSetting : IApi
    {

        public bool RequireSite => true;

        public bool RequireUser => true;

        public string ModelName => "ModuleSetting";

        [Permission(Feature.MODULE, Action = Data.Permission.Action.VIEW)]
        public string GetSetting(Guid Id, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var module = sitedb.ScriptModule.Get(Id);

            if (module != null)
            {
                return module.Settings ?? "";
            }
            return null;
        }

        [Permission(Feature.MODULE, Action = Data.Permission.Action.EDIT)]
        public void UpdateSetting(Guid Id, string json, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var module = sitedb.ScriptModule.Get(Id);

            if (module != null)
            {
                module.Settings = json;
                sitedb.ScriptModule.AddOrUpdate(module);
            }
        }

    }



}
