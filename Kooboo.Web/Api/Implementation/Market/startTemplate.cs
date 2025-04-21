using Kooboo.Api;
using Kooboo.Data.Config;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Market
{
    public class startTemplate : IApi
    {
        public string ModelName => "startTemplate";

        public bool RequireSite => false;

        public bool RequireUser => true;

        public GetSiteResult GetSite(Guid TemplateId, string rootDomain, ApiCall call)
        {
            var orgId = call.Context.User.CurrentOrgId;

            var websites = Kooboo.Data.Config.AppHost.SiteService.ListByOrg(orgId);

            var find = websites.Find(o => o.TemplateId == TemplateId);

            if (find == null)
            {
                var org = Kooboo.Data.GlobalDb.Organization.Get(orgId);
                if (org != null)
                {
                    string SiteName = GetUniqueSiteName(org.Name, orgId);

                    var fullDomain = GetUniqueFullDomain(SiteName, rootDomain);

                    find = InstallFromTemplate(TemplateId, org.Id, SiteName, fullDomain);
                }
            }

            if (find != null)
            {
                var host = find.BindingHost();
                if (!string.IsNullOrWhiteSpace(host))
                {
                    return new GetSiteResult() { Success = true, Host = host };
                }
            }
            return new GetSiteResult() { Success = false, Host = string.Empty };
        }

        private string GetUniqueFullDomain(string SubName, string RootDomain)
        {
            var fullDomain = Kooboo.Data.Config.ConfigHelper.ToFullDomain(RootDomain, SubName);

            var site = Kooboo.Data.Config.AppHost.BindingService.GetByFullDomain(fullDomain);

            int counter = 1;

            while (site != null)
            {
                counter += 1;

                fullDomain = Kooboo.Data.Config.ConfigHelper.ToFullDomain(RootDomain, SubName + counter.ToString());

                site = Kooboo.Data.Config.AppHost.BindingService.GetByFullDomain(fullDomain);

            }

            return fullDomain;

        }

        private string GetUniqueSiteName(string OrgName, Guid OrgId)
        {
            string Name = OrgName;

            var IsAvailable = Kooboo.Data.Config.AppHost.SiteService.CheckNameAvailable(OrgName, OrgId);

            int counter = 1;

            while (!IsAvailable)
            {
                counter += 1;
                Name = OrgName + counter.ToString();
                IsAvailable = Kooboo.Data.Config.AppHost.SiteService.CheckNameAvailable(Name, OrgId);
            }
            return Name;
        }

        private Kooboo.Data.Models.WebSite InstallFromTemplate(Guid TemplateId, Guid OrgId, string SiteName, string fullDomain)
        {
            var byteStream = Kooboo.Sites.Service.TemplateService.DownloadTemplate(TemplateId, OrgId);

            var site = Kooboo.Sites.Sync.ImportExport.ImportZip(byteStream, OrgId, SiteName, fullDomain, OrgId);
            site.TemplateId = TemplateId;
            AppHost.SiteRepo.AddOrUpdate(site);
            return site;

        }
    }

    public class GetSiteResult
    {
        public bool Success { get; set; }

        public string Host { get; set; }
    }
}
