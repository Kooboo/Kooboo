using Kooboo.Api;

namespace Kooboo.Web.Api.Implementation.Mails.Modules
{
    public class MailModuleSetting : IApi
    {
        public bool RequireSite => false;

        public bool RequireUser => true;

        public string ModelName => "MailModuleSetting";

        public string GetSetting(Guid Id, ApiCall call)
        {
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(Id);

            if (module != null)
            {
                return module.Settings ?? "";
            }
            return null;
        }

        public void UpdateSetting(Guid Id, string json, ApiCall call)
        {
            var orgDb = Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(Id);

            if (module != null)
            {
                module.Settings = json;
                orgDb.Module.AddOrUpdate(module);
            }
        }

    }
}