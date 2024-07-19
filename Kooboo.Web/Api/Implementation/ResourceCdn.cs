using Kooboo.Api;

namespace Kooboo.Web.Api.Implementation
{
    public class ResourceCdnApi : IApi
    {
        public string ModelName => "resourceCdn";

        public bool RequireSite => false;

        public bool RequireUser => true;


        public List<SiteResource> List(ApiCall apiCall)
        {
            var user = apiCall.Context.User;
            if (user.CurrentOrgId == default(Guid))
            {
                return null;
            }
            List<SiteResource> result = new List<SiteResource>();

            string datacenter = Data.AppSettings.ServerSetting?.DataCenter;

            var sites = Sites.Service.WebSiteService.ListByUser(user);

            foreach (var item in sites)
            {
                if (item.SiteType == Data.Definition.WebsiteType.p)
                {
                    SiteResource model = new SiteResource();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    model.DisplayName = item.DisplayName;
                    model.Enable = item.EnableResourceCDN;
                    model.DataCenter = datacenter;
                    result.Add(model);
                }
            }

            return result;

        }

        public void Enable(Guid SiteId, ApiCall call)
        {
            var site = Kooboo.Data.Config.AppHost.SiteRepo.Get(SiteId);
            if (site != null && !site.EnableResourceCDN)
            {
                site.EnableResourceCDN = true;
                Kooboo.Data.Config.AppHost.SiteRepo.AddOrUpdate(site);
            }
        }

        public void Disable(Guid SiteId, ApiCall call)
        {
            var site = Kooboo.Data.Config.AppHost.SiteRepo.Get(SiteId);
            if (site != null && site.EnableResourceCDN)
            {
                site.EnableResourceCDN = false;
                Kooboo.Data.Config.AppHost.SiteRepo.AddOrUpdate(site);
            }
        }
    }
}


public class SiteResource
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public bool Enable { get; set; }

    public string DataCenter { get; set; }
}