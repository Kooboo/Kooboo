using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Sync;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class SiteLogApi : IApi
    {
        public string ModelName
        {
            get { return "SiteLog"; }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        private int PageSize = 50;

        public PagedListViewModel<SiteLogViewModel> List(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();

            var pager = ApiHelper.GetPager(apiCall, 50);

            PagedListViewModel<SiteLogViewModel> result = new PagedListViewModel<SiteLogViewModel>();

            var total = sitedb.Log.Store.Count();
            result.TotalCount = total;
            result.TotalPages = ApiHelper.GetPageCount(total, pager.PageSize);
            result.PageNr = pager.PageNr;

            List<SiteLogViewModel> list = new List<SiteLogViewModel>();

            var items = sitedb.Log.Store.Filter.OrderByDescending().Skip(pager.SkipCount).Take(pager.PageSize);

            foreach (var item in items)
            {
                SiteLogViewModel model = new SiteLogViewModel
                {
                    LastModified = item.UpdateTime,
                    ItemName = Kooboo.Sites.Service.LogService.GetLogDisplayName(sitedb, item),
                    Id = item.Id
                };

                if (model.ItemName != null && model.ItemName.Length > 85)
                {
                    model.ItemName = Lib.Helper.StringHelper.SementicSubString(model.ItemName, 0, 70);
                    var bytes = System.Text.Encoding.UTF8.GetBytes(model.ItemName);

                    if (bytes.Length > 160)
                    {
                        model.ItemName = Lib.Helper.StringHelper.SementicSubString(model.ItemName, 0, 40);
                    }
                    model.ItemName += "...";
                }

                model.StoreName = item.StoreName;

                model.KeyHash = item.KeyHash;
                model.StoreNameHash = item.StoreNameHash;
                model.ActionType = item.EditType.ToString();

                model.UserName = Data.GlobalDb.Users.GetUserName(item.UserId);

                if (model.KeyHash != default(Guid))
                {
                    list.Add(model);
                }

            }

            result.List = list;

            return result;

        }

        public int PageCount(ApiCall apiCall)
        {
            int totalcount = apiCall.WebSite.SiteDb().Log.Store.Count();

            int pagecount = (int)totalcount / this.PageSize;

            if (totalcount > pagecount * this.PageSize)
            {
                return pagecount + 1;
            }
            return pagecount;
        }

        public List<ItemVersionViewModel> Versions(ApiCall call)
        {
            Guid KeyHash = call.GetGuidValue("KeyHash");
            int StoreNameHash = call.GetIntValue("StoreNameHash");

            if (KeyHash == default(Guid) || StoreNameHash == 0)
            {
                return null;
            }
            var logs = call.WebSite.SiteDb().Log.Store.Where(o => o.KeyHash == KeyHash && o.StoreNameHash == StoreNameHash).SelectAll();

            List<ItemVersionViewModel> list = new List<ItemVersionViewModel>();

            foreach (var item in logs.OrderByDescending(o => o.Id))
            {
                list.Add(new ItemVersionViewModel() { LastModified = item.UpdateTime, Id = item.Id, UserName = Data.GlobalDb.Users.GetUserName(item.UserId) });
            }

            return list;
        }

        public VersionCompareViewModel Compare(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            long id1 = call.GetIntValue("id1");
            long id2 = call.GetIntValue("id2");

            if (id1 == 0 && id2 == 0)
            {
                return null;
            }
            if (id1 > -1 && id2 > -1)
            {
                if (id1 > id2)
                {
                    long id3 = id1;
                    id1 = id2;
                    id2 = id3;
                }
            }
            VersionCompareViewModel model = new VersionCompareViewModel() { Id1 = id1, Id2 = id2 };

            LogEntry prelog = sitedb.Log.Get(id1);
            if (prelog != null)
            {
                var repo = sitedb.GetRepository(prelog.StoreName);
                LogEntry nextlog;
                if (id2 == -1)
                {
                    nextlog = sitedb.Log.Store.Where(o => o.KeyHash == prelog.KeyHash && o.StoreNameHash == prelog.StoreNameHash).OrderByDescending().FirstOrDefault();
                    model.Id2 = nextlog.Id;
                }
                else
                {
                    nextlog = sitedb.Log.Get(id2);
                }

                ISiteObject itemone = repo.GetByLog(prelog);
                ISiteObject itemtwo = null;
                if (nextlog.EditType != EditType.Delete)
                {
                    itemtwo = repo.GetByLog(nextlog);
                }

                model.Title1 = itemone.Name;
                model.Title2 = itemtwo != null ? itemtwo.Name : string.Empty;
                if (itemone is Image)
                {
                    string baseurl = call.WebSite.BaseUrl();

                    string url1 = (Sites.Systems.Routes.SystemRouteTemplate.Replace("{objecttype}", repo.ModelType.Name).Replace("{nameorid}", prelog.Id.ToString()));
                    string url2 = Sites.Systems.Routes.SystemRouteTemplate.Replace("{objecttype}", repo.ModelType.Name).Replace("{nameorid}", nextlog.Id.ToString());

                    model.DataType = VersionDataType.Image;
                    model.Source1 = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url1);

                    model.Source2 = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url2);
                }
                else
                {
                    model.Source1 = Sites.Service.ObjectService.GetSummaryText(itemone);
                    model.Source2 = Sites.Service.ObjectService.GetSummaryText(itemtwo);
                    model.DataType = VersionDataType.String;
                }
            }
            return model;
        }

        public bool Revert(ApiCall apiCall)
        {
            string strid = apiCall.GetValue("id");
            if (string.IsNullOrEmpty(strid))
            {
                return false;
            }

            long id = 0;
            if (long.TryParse(strid, out id))
            {
                Sites.Service.LogService.RollBack(apiCall.WebSite.SiteDb(), id);
                return true;
            }
            return false;
        }

        public bool Blame(ApiCall call)
        {
            if (string.IsNullOrEmpty(call.Context.Request.Body))
            {
                return false;
            }
            List<long> changes = Lib.Helper.JsonHelper.Deserialize<List<long>>(call.Context.Request.Body);

            if (changes != null && changes.Count() > 0)
            {
                Kooboo.Sites.Service.LogService.RollBack(call.WebSite.SiteDb(), changes);
                return true;
            }
            return false;
        }

        public bool Restore(ApiCall apicall)
        {
            string strid = apicall.GetValue("id");
            long id = -1;

            if (!string.IsNullOrEmpty(strid))
            {
                if (!long.TryParse(strid, out id))
                {
                    return false;
                }
            }
            else
            { return false; }


            if (id > -1)
            {
                Kooboo.Sites.Service.LogService.RollBackFrom(apicall.WebSite.SiteDb(), id);
                return true;
            }

            return false;
        }

        public bool CheckOut(ApiCall call)
        {
            string strsiteid = call.GetValue("SiteId");
            Guid SiteId;
            if (!System.Guid.TryParse(strsiteid, out SiteId))
            {
                return false;
            }
            var website = Kooboo.Data.GlobalDb.WebSites.Get(SiteId);
            if (website == null)
            {
                return false;
            }

            string strid = call.GetValue("id");
            long id = -1;

            if (!string.IsNullOrEmpty(strid))
            {
                if (!long.TryParse(strid, out id))
                {
                    return false;
                }
            }
            else
            { return false; }

            string subdomain = call.GetValue("SubDomain");
            string rootdomain = call.GetValue("RootDomain");
            string SiteName = call.GetValue("SiteName");

            if (string.IsNullOrEmpty(rootdomain) || string.IsNullOrEmpty(SiteName))
            {
                return false;
            }

            string fulldomain = subdomain + "." + rootdomain;

            var newwebsite = Kooboo.Sites.Service.WebSiteService.AddNewSite(website.OrganizationId, SiteName, fulldomain, call.Context.User.Id);

            Kooboo.Sites.Service.LogService.CheckOut(call.WebSite.SiteDb(), newwebsite.SiteDb(), id);

            return true;

        }

        public Guid ExportBatch(long id, ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return default(Guid);
            }
            var exportfile = ImportExport.ExportBatch(site.SiteDb(), id);

            if (string.IsNullOrWhiteSpace(exportfile))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("You have no changes", call.Context));
            }
            else
            {
                var guid = Cache.TempDownloadZip.AddPath(exportfile);

                return guid;
            }

        }

        public Guid ExportItems(List<long> ids, ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return default(Guid);
            }
            var exportfile = ImportExport.ExportBatch(site.SiteDb(), ids);

            if (string.IsNullOrWhiteSpace(exportfile))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("You have no changes", call.Context));
            }
            else
            {
                var guid = Cache.TempDownloadZip.AddPath(exportfile);

                return guid;
            }
        }

        public Guid ExportItem(long id, ApiCall call)
        {
            List<long> ids = new List<long>();
            ids.Add(id);
            return ExportItems(ids, call); 
        }

        public BinaryResponse DownloadBatch(Guid id, ApiCall call)
        {
            var site = call.WebSite;
            var path = Web.Cache.TempDownloadZip.GetPath(id);
            if (path != null && File.Exists(path))
            {
                var allbytes = System.IO.File.ReadAllBytes(path);

                BinaryResponse response = new BinaryResponse();
                response.ContentType = "application/zip";
                response.Headers.Add("Content-Disposition", $"attachment;filename={site.Name}_part.zip");
                response.BinaryBytes = allbytes;
                return response;
            }

            return null;
        }    
    }
}               