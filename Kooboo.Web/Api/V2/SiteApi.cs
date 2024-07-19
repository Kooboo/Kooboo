using System.Linq;
using Kooboo.Api;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.V2
{
    [ApiVersion(ApiVersion.V2)]
    public class SiteApi : Implementation.SiteApi
    {
        public IEnumerable<KeyValuePair<string, int>> FolderList(ApiCall call)
        {
            var orgId = call.Context.User.CurrentOrgId;
            var sites = WebSiteService.ListByUser(call.Context.User).Select(s => s.Id).ToArray();
            var folders = Kooboo.Data.Config.AppHost.SiteFolder.FolderList(orgId, sites);
            return folders.OrderBy(o => o.Key);
        }

        public void NewFolder(string name, ApiCall call)
        {
            var orgId = call.Context.User.CurrentOrgId;
            Data.Config.AppHost.SiteFolder.CreateFolder(orgId, name);
        }

        public void RemoveFolder(string name, ApiCall call)
        {
            var orgId = call.Context.User.CurrentOrgId;
            Data.Config.AppHost.SiteFolder.Delete(orgId, name);
        }

        public void RenameFolder(string name, string newName, ApiCall call)
        {
            var orgId = call.Context.User.CurrentOrgId;
            Data.Config.AppHost.SiteFolder.RenameFolder(orgId, name, newName);
        }

        private record SetSiteFolderParams(Guid[] SiteIds, string FolderName);

        public void SetSitesFolder(ApiCall call)
        {
            var @params = JsonHelper.Deserialize<SetSiteFolderParams>(call.Context.Request.Body);
            var orgId = call.Context.User.CurrentOrgId;

            foreach (var id in @params.SiteIds)
            {
                Data.Config.AppHost.SiteFolder.MoveOut(orgId, id);

                if (!string.IsNullOrWhiteSpace(@params.FolderName))
                {
                    Data.Config.AppHost.SiteFolder.MoveIn(orgId, id, @params.FolderName);
                }
            }
        }



        private List<SiteSummaryViewModel> SimpleList(ApiCall apiCall)
        {
            var user = apiCall.Context.User;
            if (user.CurrentOrgId == default(Guid))
            {
                return null;
            }

            var sites = Kooboo.Sites.Service.WebSiteService.ListByUser(user);

            List<SiteSummaryViewModel> result = new List<SiteSummaryViewModel>();

            foreach (var item in sites)
            {

                SiteSummaryViewModel summary = new SiteSummaryViewModel();
                summary.SiteId = item.Id;
                summary.SiteName = item.Name;
                summary.SiteDisplayName = item.DisplayName;
                //  summary.PageCount = sitedb.Pages.Count();
                //  summary.ImageCount = sitedb.Images.Count();
                // if user has not right to access the site. present the preview link.  

                summary.Online = item.Published;
                //  summary.Visitors = sitedb.VisitorLog.QueryDescending(o => true).EndQueryCondition(o => o.Begin < DateTime.UtcNow.AddHours(-12)).Count();
                // NewMethod( summary);

                result.Add(summary);
            }
            return result;
        }

        private static void ExtendInfo(SiteSummaryViewModel summary)
        {
            try
            {
                var site = Kooboo.Data.Config.AppHost.SiteRepo.Get(summary.SiteId);

                var sitedb = site.SiteDb();
                var alltask = sitedb.TransferTasks.All();

                if (alltask != null && alltask.Count() > 0)
                {
                    var tasks = alltask.FindAll(o => o != null && o.done == false);

                    foreach (var ttask in tasks)
                    {
                        if (ttask.CreationDate > DateTime.UtcNow.AddMinutes(-2))
                        {
                            summary.InProgress = true;
                            break;
                        }
                    }
                }

                var defaultRoute = ObjectRoute.GetDefaultRoute(sitedb);
                if (defaultRoute != null && (site.BaseUrl()?.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    summary.HomePageLink = site.BaseUrl().TrimEnd('/') + defaultRoute.Name;
                }

                summary.PageCount = sitedb.Pages.Count();
                summary.ImageCount = sitedb.Images.Count();
            }
            catch (Exception ex)
            {
                summary.HomePageLink = ex.Message;
            }

        }

        public PagedListViewModel<SiteSummaryViewModel> PagedList(ApiCall call)
        {
            var pager = ApiHelper.GetPager(call, 50);
            var result = new PagedListViewModel<SiteSummaryViewModel>();

            IEnumerable<SiteSummaryViewModel> sites = SimpleList(call);

            var orgId = call.Context.User.CurrentOrgId;
            var folder = call.GetValue("folder");

            if (!string.IsNullOrWhiteSpace(folder))
            {
                var siteIds = Data.Config.AppHost.SiteFolder.ListByFolder(orgId, folder);
                sites = sites.Where(w => siteIds.Contains(w.SiteId)).ToArray();
            }
            else
            {
                var exSiteIds = Data.Config.AppHost.SiteFolder.AllSitesInFolders(orgId);
                sites = sites.Where(w => !exSiteIds.Contains(w.SiteId)).ToArray();
            }


            var keyword = call.GetValue("keyword")?.ToLower()?.Trim();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sites = sites.Where(w =>
                    (w.SiteName?.ToLower().Contains(keyword) ?? false)
                    || (w.SiteDisplayName?.ToLower().Contains(keyword) ?? false)
                ).ToList();
            }

            result.TotalCount = sites.Count();
            result.TotalPages = ApiHelper.GetPageCount(result.TotalCount, pager.PageSize);
            result.PageNr = pager.PageNr;
            result.PageSize = pager.PageSize;


            result.List = sites.Skip(pager.SkipCount).Take(pager.PageSize).ToList();


            foreach (var item in result.List)
            {
                //AccessControl.GrantDevAccess(call.Context, item.SiteId);
                ExtendInfo(item);
            }

            return result;
        }

        public IEnumerable<KeyValuePair<Guid, string>> KeyValueList(ApiCall call)
        {
            var sites = Kooboo.Sites.Service.WebSiteService.ListByUser(call.Context.User);

            return sites.Select(s => new KeyValuePair<Guid, string>(s.Id, s.DisplayName));
        }
    }
}