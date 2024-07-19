//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Linq;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Config;
using Kooboo.Data.Interface;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Data.Permission;
using Kooboo.Data.Service;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Sites.Sync;
using Kooboo.Web.ViewModel;

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

        [Permission(Feature.SITE, Action = "log")]
        public PagedListViewModel<SiteLogViewModel> List(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();

            var pager = ApiHelper.GetPager(apiCall, 50);

            List<SiteLogViewModel> list = new List<SiteLogViewModel>();

            var all = sitedb.Log.Store.Filter.OrderByDescending().SelectAll();
            all = Kooboo.Web.Service.LogService.Filter(all, apiCall);
            PagedListViewModel<SiteLogViewModel> result = new PagedListViewModel<SiteLogViewModel>
            {
                TotalCount = all.Count,
                TotalPages = ApiHelper.GetPageCount(all.Count, pager.PageSize),
                PageNr = pager.PageNr,
                PageSize = pager.PageSize
            };
            var items = all.Skip(pager.SkipCount).Take(pager.PageSize);

            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }
                SiteLogViewModel model = new SiteLogViewModel();

                model.LastModified = item.UpdateTime;

                try
                {
                    model.ItemName = Kooboo.Sites.Service.LogService.GetLogDisplayName(sitedb, item);
                    model.Id = item.Id;
                }
                catch (Exception ex)
                {
                    Kooboo.Data.Log.Instance.Exception.WriteException(ex);
                }

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

                if (!string.IsNullOrEmpty(item.StoreName))
                {
                    model.StoreName = item.StoreName;
                }
                else if (item.IsTable)
                {
                    model.StoreName = Kooboo.Data.Language.Hardcoded.GetValue("Table", apiCall.Context);
                }

                model.KeyHash = item.KeyHash;
                model.StoreNameHash = item.StoreNameHash;
                model.TableNameHash = item.TableNameHash;
                model.TableName = item.TableName;

                model.ActionType = item.EditType.ToString();

                model.UserName = Data.GlobalDb.Users.GetUserName(item.UserId);

                if (model.KeyHash != default(Guid))
                {
                    model.HasVideo = SiteLogVideoService.Exist(apiCall.Context.WebSite, model.Id);
                    list.Add(model);
                }

            }

            result.List = list;

            return result;

        }

        [Permission(Feature.SITE, Action = "log")]
        public Dictionary<string, IEnumerable<KeyValuePair<string, string>>> Options(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();

            var all = sitedb.Log.Store.Filter.SelectAll();
            var storeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var editTypes = new HashSet<EditType>();
            var userMaps = new Dictionary<string, string>();

            foreach (var item in all)
            {
                if (item != null)
                {
                    storeNames.Add(string.IsNullOrEmpty(item.StoreName) && item.IsTable ? "Table" : item.StoreName);
                    editTypes.Add(item.EditType);
                    if (item.UserId != default && !userMaps.ContainsKey(item.UserId.ToString()))
                    {
                        userMaps[item.UserId.ToString()] = Data.GlobalDb.Users.GetUserName(item.UserId);
                    }
                }
            }

            return new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>
            {
                [nameof(LogEntry.EditType)] = editTypes.OrderBy(o => (int)o).Select(s => s.ToString())
                .Select(it => new KeyValuePair<string, string>(it, Hardcoded.GetValue(it, apiCall.Context))).ToArray(),
                [nameof(LogEntry.StoreName)] = storeNames.ToArray()
                .Select(it => new KeyValuePair<string, string>(it, Hardcoded.GetValue(it, apiCall.Context))).OrderBy(it => it.Key),
                [nameof(User)] = userMaps.ToList().OrderBy(it => it.Value)
            };
        }

        [Permission(Feature.SITE, Action = "log")]
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

        [Permission(Feature.SITE, Action = "log")]
        public List<ItemVersionViewModel> Versions(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            Guid KeyHash = call.GetValue<Guid>("KeyHash");
            int StoreNameHash = call.GetValue<int>("StoreNameHash");
            int TableNameHash = call.GetValue<int>("TableNameHash");

            if (KeyHash == default(Guid) || (StoreNameHash == 0 && TableNameHash == 0))
            {
                return null;
            }

            List<LogEntry> logs = null;
            if (StoreNameHash == 0)
            {
                logs = sitedb.Log.Store.Where(o => o.KeyHash == KeyHash && o.TableNameHash == TableNameHash).SelectAll();
            }
            else
            {
                logs = sitedb.Log.Store.Where(o => o.KeyHash == KeyHash && o.StoreNameHash == StoreNameHash).SelectAll();
            }

            List<ItemVersionViewModel> list = new List<ItemVersionViewModel>();
            foreach (var item in logs.OrderByDescending(o => o.Id))
            {
                list.Add(new ItemVersionViewModel()
                {
                    LastModified = item.UpdateTime,
                    Id = item.Id,
                    UserName = Data.GlobalDb.Users.GetUserName(item.UserId),
                    HasVideo = SiteLogVideoService.Exist(call.Context.WebSite, item.Id)
                });
            }

            return list;
        }

        [Permission(Feature.SITE, Action = "log")]
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

            LogEntry prelog = sitedb.Log.Get(id1);
            if (prelog != null)
            {
                if (prelog.IsTable)
                {
                    return GetTableCompareModel(call, sitedb, id1, id2);
                }
                else
                {
                    return GetStoreCompareModel(call, sitedb, id1, id2);
                }
            }

            return new VersionCompareViewModel();
        }

        [Permission(Feature.SITE, Action = "log")]
        private VersionCompareViewModel GetStoreCompareModel(ApiCall call, Sites.Repository.SiteDb sitedb, long id1, long id2)
        {
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

        [Permission(Feature.SITE, Action = "log")]
        private VersionCompareViewModel GetTableCompareModel(ApiCall call, Sites.Repository.SiteDb sitedb, long id1, long id2)
        {
            VersionCompareViewModel model = new VersionCompareViewModel() { Id1 = id1, Id2 = id2 };

            LogEntry preLog = sitedb.Log.Get(id1);
            if (preLog != null)
            {
                var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);
                var table = Data.DB.GetTable(db, preLog.TableName);

                LogEntry nextLog;
                if (id2 == -1)
                {
                    nextLog = sitedb.Log.Store.Where(o => o.KeyHash == preLog.KeyHash && o.TableNameHash == preLog.TableNameHash).OrderByDescending().FirstOrDefault();
                    model.Id2 = nextLog.Id;
                }
                else
                {
                    nextLog = sitedb.Log.Get(id2);
                }
                var itemOne = table.GetLogData(preLog);
                Dictionary<string, object> itemTwo = null;

                if (nextLog.EditType != EditType.Delete)
                {
                    itemTwo = table.GetLogData(nextLog);
                }

                model.Title1 = Data.Language.Hardcoded.GetValue("Table", call.Context) + ":" + preLog.TableName;
                model.Title2 = model.Title1;

                if (itemOne != null)
                {
                    model.Source1 = Sites.Service.ObjectService.GetSummaryText(itemOne);
                }
                if (itemTwo != null)
                {
                    model.Source2 = Sites.Service.ObjectService.GetSummaryText(itemTwo);
                }
                model.DataType = VersionDataType.String;
            }
            return model;
        }

        [Permission(Feature.SITE, Action = "log")]
        public bool Revert(long id, ApiCall apiCall)
        {
            Sites.Service.LogService.RollBack(apiCall.WebSite.SiteDb(), id);
            return true;
        }

        [Permission(Feature.SITE, Action = "log")]
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

        [Permission(Feature.SITE, Action = "log")]
        public bool Restore(long id, ApiCall apicall)
        {
            if (id > -1)
            {
                Kooboo.Sites.Service.LogService.RollBackFrom(apicall.WebSite.SiteDb(), id);
                return true;
            }

            return false;
        }

        [Permission(Feature.SITE, Action = "log")]
        public bool CheckOut(Guid SiteId, long Id, ApiCall call)
        {
            var website = Data.Config.AppHost.SiteRepo.Get(SiteId);
            if (website == null)
            {
                return false;
            }

            string subdomain = call.GetValue("SubDomain");
            string rootdomain = call.GetValue("RootDomain");
            string SiteName = call.GetValue("SiteName");

            if (string.IsNullOrEmpty(rootdomain) || string.IsNullOrEmpty(SiteName))
            {
                return false;
            }

            string fulldomain = subdomain + "." + rootdomain;

            var newwebsite = Kooboo.Sites.Service.WebSiteService.AddNewSite(website.OrganizationId, SiteName, fulldomain, call.Context.User.Id, false);

            try
            {
                Kooboo.Sites.Service.LogService.CheckOut(call.WebSite.SiteDb(), newwebsite.SiteDb(), Id);
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                newwebsite.Published = true;
                AppHost.SiteRepo.AddOrUpdate(newwebsite);
            }
        }

        [Permission(Feature.SITE, Action = "log")]
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

        [Permission(Feature.SITE, Action = "log")]
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

        [Permission(Feature.SITE, Action = "log")]
        public Guid ExportItem(long id, ApiCall call)
        {
            List<long> ids = new List<long>();
            ids.Add(id);
            return ExportItems(ids, call);
        }

        [Permission(Feature.SITE, Action = "log")]
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

        public void SaveLogVideo(Guid id, string videoData, ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            var userId = call.Context.User.Id;
            var isEmbedded = call.GetBoolValue("isEmbedded");
            if (isEmbedded)
            {
                id = GetOwnerId(siteDb, id);
                if (id == default) return;
            }
            id = Sites.Service.LogService.GetKeyHash(id);
            var logEntry = siteDb.Log.Store.FullScan(w => w.KeyHash == id && w.UserId == userId).OrderByDescending().Take(1).FirstOrDefault();
            SiteLogVideoService.Save(call.Context.WebSite, logEntry.Id.ToString(), videoData);
        }

        private Guid GetOwnerId(SiteDb siteDb, Guid id)
        {
            var code = siteDb.Code.Get(id);
            if (code != default)
            {
                return code.OwnerObjectId;
            }
            var style = siteDb.Styles.Get(id);
            if (style != default)
            {
                return style.OwnerObjectId;
            }

            var script = siteDb.Scripts.Get(id);
            if (script != default)
            {
                return script.OwnerObjectId;
            }

            var form = siteDb.Forms.Get(id);
            if (form != default)
            {
                return form.OwnerObjectId;
            }

            return default;
        }

        public string GetLogVideo(long versionId, ApiCall call)
        {
            return SiteLogVideoService.Get(call.Context.WebSite, versionId);
        }

        [Permission(Feature.SITE, Action = "log")]
        public Guid CleanLog(ApiCall call)
        {
            return Kooboo.Sites.Service.CleanerService.AddCleanLogTask(call.Context.WebSite);
        }

        private void ReplaceTest(SiteDb sitedb)
        {
            Dictionary<string, string> replaceCase = new Dictionary<string, string>();
            replaceCase.Add("23 Digital Pty. Ltd. Australia", "ZOFELA");
            replaceCase.Add("23 Digital Pty. Ltd.", "ZOFELA");
            replaceCase.Add("23 Digital", "ZOFELA");
            replaceCase.Add("23 digital", "ZOFELA");

            //Melbourne Sydney Brisbane  Australia
            //replaceCase.Add(" Melbourne", " China");
            //replaceCase.Add(" Sydney", " China");
            //replaceCase.Add(" Brisbane", " China");
            //replaceCase.Add(" Australia", " China");

            List<string> cities = new List<string>();
            cities.Add("Melbourne");
            cities.Add("Sydney");
            cities.Add("Brisbane");
            cities.Add("Australia");

            Dictionary<string, string> cityReplace = new Dictionary<string, string>();

            foreach (var item in cities)
            {
                cityReplace.Add(" " + item + ",", " China,");
                cityReplace.Add(" " + item + " ", " China ");
                cityReplace.Add(" " + item + ".", " China.");
                cityReplace.Add(" " + item + "?", " China?");
                cityReplace.Add(">" + item + " ", ">China ");
                cityReplace.Add(">" + item + ",", ">China,");
                cityReplace.Add(">" + item + ".", ">China.");
                cityReplace.Add(">" + item + "?", ">China?");

                cityReplace.Add(" " + item + "\"", " China\"");
                cityReplace.Add(" " + item + "'", " China'");
                cityReplace.Add(" " + item + "!", " China!");

                cityReplace.Add("\"" + item + " ", "\"China ");

                cityReplace.Add("\"" + item + "\"", " \"China\"");
                cityReplace.Add(item + "</", "China</");
            }


            var pages = sitedb.Pages.All();

            foreach (var item in pages)
            {
                var orgLen = item.Body.Length;

                var body = item.Body;

                foreach (var text in cityReplace)
                {
                    body = body.Replace(text.Key, text.Value);
                }

                var newLen = body.Length;

                if (orgLen != newLen)
                {
                    item.Body = body;

                    sitedb.Pages.Store.update(item.Id, item);
                }
            }

        }

        [Permission(Feature.SITE, Action = "log")]
        public CleanLogTaskResult CleanLogStatus(Guid TaskId, ApiCall call)
        {
            return Kooboo.Sites.Service.CleanerService.GetCleanLogTaskStatus(TaskId);
        }

        public CleanLogRunningTask GetCleanLogRunningTask(ApiCall call)
        {
            return Kooboo.Sites.Service.CleanerService.GetRunningTask(call.Context.WebSite.Id);
        }

    }
}