using System.Linq;
using System.Threading;
using Google.Apis.Webmasters.v3.Data;
using Kooboo.Api;
using Kooboo.Data.Helper;
using Kooboo.Lib.Helper;
using ApiHelper = Kooboo.Data.Helper.ApiHelper;

namespace Kooboo.Web.Api.Implementation;

public class SearchConsole : IApi
{
    public string ModelName => "SearchConsole";
    public bool RequireSite => false;
    public bool RequireUser => true;

    public string GetAuthUrl(ApiCall call, string callbackUrl)
    {
        var headers = ApiHelper.GetAuthHeaders(call.Context);

        var body = JsonHelper.Serialize(new
        {
            callbackUrl = callbackUrl
        });

        return HttpHelper.Post<string>(AccountUrlHelper.SearchConsole("GetAuthUrl"), body, headers);
    }

    public record ScSite(string SiteUrl, string PermissionLevel, bool onGoogle);

    public IList<ScSite> GetSiteList(ApiCall call)
    {
        var domains = new DomainApi().List(call).Where(w => w.EnableDns).ToArray() ?? [];

        var headers = ApiHelper.GetAuthHeaders(call.Context);

        //google api calls often fail on kooboo account server ,retry 3 times
        var retryCount = 3;
        SitesListResponse result = null;

        while (retryCount > 0)
        {
            try
            {
                result = HttpHelper.Post<SitesListResponse>(
                    AccountUrlHelper.SearchConsole("GetSiteList"),
                    null,
                    headers,
                    true);
                if (result != default) retryCount = 0;
                else throw new Exception();
            }
            catch (System.Exception)
            {
                retryCount--;
                Thread.Sleep(300);
            }
        }

        if (result == default) throw new Exception("Connect to google api error");

        var remoteList = result?.SiteEntry ?? Array.Empty<WmxSite>();
        var scSites = new List<ScSite>();

        foreach (var item in domains)
        {
            var domainName = $"sc-domain:{item.DomainName}";
            var exist = remoteList.FirstOrDefault(f => f.SiteUrl.Contains(item.DomainName));
            var permissionLevel = exist?.PermissionLevel ?? "siteUnverifiedUser";
            scSites.Add(new ScSite(exist?.SiteUrl ?? domainName, permissionLevel, exist != default));
        }
        
        return scSites;
    }

    public SearchAnalyticsQueryResponse GetSearchAnalytics(ApiCall call)
    {
        var headers = ApiHelper.GetAuthHeaders(call.Context);

        var result = HttpHelper.Post<SearchAnalyticsQueryResponse>(
            AccountUrlHelper.SearchConsole("GetSearchAnalytics"),
            call.Context.Request.Body,
            headers);

        return result;
    }

    public void AddSite(ApiCall call, string siteUrl)
    {
        var headers = ApiHelper.GetAuthHeaders(call.Context);
        var body = JsonHelper.Serialize(new
        {
            siteUrl = siteUrl
        });
        HttpHelper.Post<string>(AccountUrlHelper.SearchConsole("AddSite"), body, headers);
    }

    public void DeleteSite(ApiCall call, string siteUrl)
    {
        var headers = ApiHelper.GetAuthHeaders(call.Context);
        var body = JsonHelper.Serialize(new
        {
            siteUrl = siteUrl
        });
        HttpHelper.Post<string>(AccountUrlHelper.SearchConsole("DeleteSite"), body, headers);
    }

    public void ValidSite(ApiCall call, string siteUrl)
    {
        var onGoogle = call.GetBoolValue("onGoogle");
        if (!onGoogle)
        {
            AddSite(call, siteUrl);
        }
        var headers = ApiHelper.GetAuthHeaders(call.Context);
        var body = JsonHelper.Serialize(new
        {
            siteUrl = siteUrl
        });
        HttpHelper.Post<string>(AccountUrlHelper.SearchConsole("ValidSite"), body, headers);
    }

    public IList<WmxSitemap> GetSitemapList(ApiCall call, string siteUrl)
    {
        var headers = ApiHelper.GetAuthHeaders(call.Context);

        var body = JsonHelper.Serialize(new
        {
            siteUrl = siteUrl
        });

        var result = HttpHelper.Post<SitemapsListResponse>(
            AccountUrlHelper.SearchConsole("GetSitemapList"),
            body,
            headers
        );

        return result?.Sitemap ?? Array.Empty<WmxSitemap>();
    }

    public void SubmitSitemap(ApiCall call, string siteUrl, string feedPath)
    {
        var headers = ApiHelper.GetAuthHeaders(call.Context);

        var body = JsonHelper.Serialize(new
        {
            siteUrl = siteUrl,
            feedPath = feedPath
        });

        HttpHelper.Post<SitesListResponse>(
            AccountUrlHelper.SearchConsole("SubmitSitemap"),
            body,
            headers
        );
    }

    public void DeleteSitemap(ApiCall call, string siteUrl, string feedPath)
    {
        var headers = ApiHelper.GetAuthHeaders(call.Context);

        var body = JsonHelper.Serialize(new
        {
            siteUrl = siteUrl,
            feedPath = feedPath
        });

        HttpHelper.Post<SitesListResponse>(
            AccountUrlHelper.SearchConsole("DeleteSitemap"),
            body,
            headers
        );
    }
}