//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Permission;

namespace Kooboo.Web.ViewModel
{
    public class SiteCultureViewModel
    {
        public SiteCultureViewModel()
        {
            this.Cultures = new Dictionary<string, string>();
        }
        public string Default { get; set; }

        public string DefaultName { get; set; }

        public Dictionary<string, string> Cultures;
    }

    public class SiteSummaryViewModel
    {
        public SiteSummaryViewModel()
        {

        }

        public SiteSummaryViewModel(WebSite site)
        {
            var sitedb = site.SiteDb();
            this.SiteId = site.Id;
            this.SiteName = site.Name;
            this.SiteDisplayName = site.DisplayName;
            this.PageCount = sitedb.Pages.Count();
            this.ImageCount = sitedb.Images.Count();
            this.Online = site.Published;
            //summary.Visitors = sitedb.VisitorLog.QueryDescending(o => true).EndQueryCondition(o => o.Begin < DateTime.UtcNow.AddHours(-12)).Count();

            var alltask = sitedb.TransferTasks.All();

            if (alltask != null && alltask.Count() > 0)
            {
                var tasks = alltask.FindAll(o => o != null && o.done == false);

                foreach (var ttask in tasks)
                {
                    if (ttask.CreationDate > DateTime.UtcNow.AddMinutes(-2))
                    {
                        this.InProgress = true;
                        break;
                    }
                }
            }

            this.HomePageLink = site.BaseUrl();

        }

        public Guid SiteId { get; set; }
        public string SiteName { get; set; }

        public string SiteDisplayName { get; set; }

        public int PageCount { get; set; }

        public int ImageCount { get; set; }

        public int Visitors { get; set; }

        public bool InProgress { get; set; }

        public string HomePageLink { get; set; }

        public string EditPageLink { get; set; }

        public bool Online { get; set; }

    }

    public class SiteDiskSyncViewModel
    {
        public string Folder { get; set; }
        public bool EnableDiskSync { get; set; }

        public int DiskFileCount { get; set; }

    }

    public class CultureInfoViewModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

    }

    public class SiteBindingViewModel
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public int BindingCount { get; set; }
    }

    public class ExportStoreNameViewModel
    {

        public ExportStoreNameViewModel(Type coreObjectType, RenderContext context)
        {
            this.Name = coreObjectType.Name;
            this.DisplayName = Kooboo.Data.Language.Hardcoded.GetValue(this.Name, context);
        }

        public ExportStoreNameViewModel()
        { }

        public ExportStoreNameViewModel(string storeName, string displayName)
        {
            this.Name = storeName;
            this.DisplayName = displayName;
        }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<string> Related { get; set; }
    }


    public class SiteUpdate : WebSite
    {
        public List<string> Clusters { get; set; }

        public override Dictionary<string, string> Culture
        {
            get; set;
        }
    }

    public class SiteViewModel
    {
        public WebSite Site { get; set; }
        public List<PermissionItem> Permissions { get; set; }
        public bool IsAdmin { get; set; }

        public string BaseUrl { get; set; }
        public int ServiceLevel { get; set; }

        public bool? ShowContinueDownload { get; set; }

        public string PrUrl { get; set; }
        public string[] VisibleAdvancedMenus { get; set; }
        public ModuleMenu[] ModuleMenus { get; set; } = [];
    }

    public class ModuleMenu
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
