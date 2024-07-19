using Kooboo.Api;
using Kooboo.Data.Config;

namespace Kooboo.Web.Api.Implementation;

public class Sitemap : IApi
{
    public string ModelName => "sitemap";
    public bool RequireSite => false;
    public bool RequireUser => true;

    public string[] Feeds(ApiCall call, string domain)
    {
        call.Context.User.ThrowIfNotAdmin();
        var currentOrgId = call.Context.User.CurrentOrgId;

        var bindings = AppHost.BindingRepo.LoadOrg(currentOrgId);

        var result = new List<string>();

        foreach (var binding in bindings)
        {
            if (binding.FullDomain == null) continue;
            if (!binding.FullDomain.EndsWith(domain)) continue;
            var webSite = AppHost.SiteRepo.Get(currentOrgId, binding.WebSiteId);
            if (!webSite.SitemapSettings.Enable) continue;

            if (!webSite.ForceSSL)
            {
                result.Add($"http://{binding.FullDomain}/{webSite.SitemapSettings.Path?.Trim('/')}");
            }

            result.Add($"https://{binding.FullDomain}/{webSite.SitemapSettings.Path?.Trim('/')}");
        }

        return result.ToArray();
    }
}