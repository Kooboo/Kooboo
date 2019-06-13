//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
    public static class WebSiteService
    {
        public static void Delete(Guid WebSiteId, User user = null)
        {
            var website = Data.GlobalDb.WebSites.Get(WebSiteId);
            if (website != null)
            {
                if (user != null && user.CurrentOrgId != website.OrganizationId)
                {
                    // has no right to delete. 
                    return;
                }

                // make sure not request to website can be made any more...
                // We need to wait till all request is finished! How????? 
                GlobalDb.WebSites.Delete(WebSiteId);

                Thread.Sleep(20);

                var sitedb = website.SiteDb();

                var clsmanager = sitedb.ClusterManager;
                if (clsmanager != null)
                {
                    clsmanager.PushQueue.Clear();
                    clsmanager = null;
                }

                sitedb.ImagePool.ClearAll(); 
                //website.Locked = true; 
                website.EnableDiskSync = false;
                website.EnableFrontEvents = false;
                website.EnableSitePath = false;
                website.EnableVisitorLog = false;
                website.ContinueDownload = false;
                website.Published = false;

                if (Kooboo.Data.DB.HasKDatabase(website))
                {
                    var kdb = Kooboo.Data.DB.GetKDatabase(website);
                    kdb.deleteDatabase();
                }

                sitedb.SearchIndex.DelSelf();
                sitedb.ImagePool.ClearAll();
                sitedb.VisitorLog.DelSelf();
                sitedb.ImageLog.DelSelf();
                sitedb.ErrorLog.DelSelf();
                sitedb.DatabaseDb.Close();
 
                sitedb.DatabaseDb.deleteDatabase();

                if (Kooboo.Data.DB.HasKDatabase(website))
                {
                    var kdb = Kooboo.Data.DB.GetKDatabase(website);
                    kdb.deleteDatabase(); 
                }
                 
                Thread.Sleep(20);

                Cache.WebSiteCache.RemoveWebSitePlan(website.Id);

                var folder = website.DiskSyncFolder;

                if (System.IO.Directory.Exists(folder))
                {
                    try
                    {
                        System.IO.Directory.Delete(folder, true);
                    }
                    catch (Exception)
                    {
                        Kooboo.Sites.TaskQueue.QueueManager.Add(new Kooboo.Sites.TaskQueue.Model.DeleteDisk() { FullPath = folder, IsFolder = true }, WebSiteId);
                    }
                }

                website = null;
                Cache.WebSiteCache.Remove(sitedb);
            }
        }

        public static HashSet<string> GetCultureFromContentRepo(WebSite website)
        {
            HashSet<string> cultures = new HashSet<string>();

            var allcontents = website.SiteDb().TextContent.All();
            foreach (var item in allcontents)
            {
                foreach (var langcontent in item.Contents)
                {
                    if (langcontent != null && langcontent.Lang != null)
                    {
                        cultures.Add(langcontent.Lang);
                    }
                }
            }
            return cultures;
        }

        public static void InitSiteCultureAfterImport(WebSite website)
        {
            var culture = GetDefaultCultureFromContent(website);
            if (culture != null)
            {
                website.DefaultCulture = culture;
                GlobalDb.WebSites.AddOrUpdate(website);
                return;
            }
        }

        private static string GetDefaultCultureFromContent(WebSite website)
        {
            Dictionary<string, int> contentcounter = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var allcontents = website.SiteDb().TextContent.All();
            foreach (var item in allcontents)
            {
                foreach (var langcontent in item.Contents)
                {
                    if (contentcounter.ContainsKey(langcontent.Lang))
                    {
                        var currentcount = contentcounter[langcontent.Lang];
                        currentcount += 1;
                        contentcounter[langcontent.Lang] = currentcount;
                    }
                    else
                    {
                        contentcounter.Add(langcontent.Lang, 1);
                    }
                }
            }

            if (contentcounter.Count() > 0)
            {
                var keyvalue = contentcounter.OrderByDescending(o => o.Value).First();
                return keyvalue.Key;
            }
            return null;
        }

        public static void InitDiskSync(WebSite website, bool newThread = false)
        {
            lock (Sites.TaskQueue.DiskSyncWorker._locker)
            {
                var manager = Sync.DiskSyncHelper.GetSyncManager(website.Id);

                if (newThread)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(manager.InitSyncToDisk);
                }
                else
                {
                    var sitedb = website.SiteDb();
                    var allrepos = sitedb.ActiveRepositories();
                    foreach (var repo in allrepos)
                    {
                        if (Kooboo.Attributes.AttributeHelper.IsDiskable(repo.ModelType))
                        {
                            var allitems = repo.All();

                            foreach (var item in allitems)
                            {
                                manager.SyncToDisk(sitedb, item, ChangeType.Add, repo.StoreName);
                            }
                        }
                    }
                }
            }
        }

        public static string GetCustomErrorUrl(WebSite website, int statusCode)
        {
            if (website.CustomErrors.ContainsKey(statusCode))
            {
                var page = website.CustomErrors[statusCode];
                Guid PageId = default(Guid);
                if (System.Guid.TryParse(page, out PageId))
                {
                    return ObjectService.GetObjectRelativeUrl(website.SiteDb(), PageId, ConstObjectType.Page);
                }
                else
                {
                    return page;
                }
            }
            else
            {
                if (statusCode == 404)
                {
                    return DataConstants.Default404Page;
                }
                else if (statusCode == 500)
                {
                    return DataConstants.Default500Page;
                }
                else if (statusCode == 403)
                {
                    return DataConstants.Default403Page; 
                }
                else if (statusCode == 407)
                {
                    return DataConstants.Default407Page; 
                }

                else if (statusCode == 402)
                {
                    return DataConstants.Default402Page; 
                }

                return DataConstants.DefaultError;
            }

        }


        public static List<WebSite> ListByUser(User user)
        {
            var sites = Kooboo.Data.GlobalDb.WebSites.ListByOrg(user.CurrentOrgId);  

            if (user.IsAdmin)
            {
                return sites; 
            }

            List<WebSite> ownsites = new List<WebSite>();
            foreach (var item in sites)
            {

                var sitedb = item.SiteDb(); 
                if (sitedb.SiteUser.All().Find(o=>o.UserId == user.Id) !=null)
                {
                    ownsites.Add(item); 
                }

            }

            return ownsites; 

        }

        public static WebSite AddNewSite(Guid organizationId, string SiteName,string FullDomain, Guid UserId)
        {
            var newsite = GlobalDb.WebSites.AddNewWebSite(organizationId, SiteName, FullDomain);
            
            if (!GlobalDb.Users.IsAdmin(organizationId, UserId))
            {
                string name = null; 

                var user = GlobalDb.Users.Get(UserId); 
                 if(user !=null)
                {
                    name = user.UserName; 
                }

                var sitedb = newsite.SiteDb();
                sitedb.SiteUser.AddOrUpdate(new Models.SiteUser() { Id = UserId, UserId = UserId, Role = Authorization.EnumUserRole.SiteMaster, Name = name });  
            }

            return newsite; 
        }

        public static async Task<string> RenderCustomError(FrontContext context, int statusCode)
        {
            var url = GetCustomErrorUrl(context.WebSite, statusCode);
            return await RenderCustomErrorPage(context, url);  
        }

        public static async Task<string> RenderCustomErrorPage(FrontContext context, string relativeUrl)
        {
            string rawcontent = null; 

            if (relativeUrl == DataConstants.Default403Page || relativeUrl == DataConstants.Default407Page || relativeUrl == DataConstants.Default404Page || relativeUrl == DataConstants.Default402Page || relativeUrl == DataConstants.Default500Page || relativeUrl == DataConstants.DefaultError)
            {
                var filename = Lib.Compatible.CompatibleManager.Instance.System.CombinePath(AppSettings.RootPath, relativeUrl);
                var extension = System.IO.Path.GetExtension(filename);
                if (!".html".Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    filename += ".html";
                }
                if (System.IO.File.Exists(filename))
                {
                    return System.IO.File.ReadAllText(filename); 
                }
            }
            else 
            {
                var route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(context.SiteDb, relativeUrl);
                if (route != null)
                {
                    context.Route = route;
                    await Kooboo.Sites.Render.RouteRenderers.RenderAsync(context);
                    if (context.RenderContext.Response.Body != null && context.RenderContext.Response.Body.Length > 0)
                    {
                        rawcontent = System.Text.Encoding.UTF8.GetString(context.RenderContext.Response.Body);
                    }
                }

                if (!string.IsNullOrEmpty(rawcontent))
                {
                    string baseurl = context.RenderContext.WebSite.BaseUrl(relativeUrl);
                    string result = Kooboo.Sites.Service.HtmlHeadService.SetBaseHref(rawcontent, baseurl);
                    return result; 
                }
            }
             
            return "Internal Server Error"; 
        }
 

    }
}
