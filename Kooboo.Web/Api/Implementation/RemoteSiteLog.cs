//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Data.Service;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class RemoteSiteLogApi : IApi
    {
        public string ModelName
        {
            get { return "RemoteSiteLog"; }
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
                return false;
            }
        }

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

        public string GetLogVideo(long versionId, ApiCall call)
        {
            return SiteLogVideoService.Get(call.Context.WebSite, versionId);
        }

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
    }
}