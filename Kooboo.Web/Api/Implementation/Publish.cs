//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Config;
using Kooboo.Data.Context.UserProviders;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Data.Permission;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Sites.Sync;
using Kooboo.Sites.Sync.Settings;
using Kooboo.Sites.ViewModel;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class PublishApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "publish";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }


        public List<SimpleSiteItemViewModel> SiteList(ApiCall call)
        {
            User user = call.Context.User;
            if (user == null)
            {
                return null;
            }

            var OrgId = call.GetValue<Guid>("OrganizationId");
            if (OrgId == default(Guid))
            {
                OrgId = user.CurrentOrgId;
            }

            var sites = WebSiteService.RemoteListByUser(user, OrgId);

            List<SimpleSiteItemViewModel> result = new List<SimpleSiteItemViewModel>();

            foreach (var item in sites)
            {
                var lastVersion = item.SiteDb().Log.Store.LastKey;

                result.Add(new SimpleSiteItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    LastVersion = lastVersion,
                    Url = item.BaseUrl()?.TrimEnd('/')
                });
            }

            return result;
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public List<SimpleSiteItemViewModel> RemoteSiteList(string remoteUrl, Guid orgId, ApiCall call)
        {
            var model = UserPublishService.RemoteSiteList(remoteUrl, orgId, call.Context);
            var list = call.Context.WebSite.SiteDb()
                .SyncSettings.All()
                .Select(s => s.RemoteWebSiteId)
                .Union([call.WebSite.Id]);

            model = model.Where(w => !list.Contains(w.Id)).ToList();
            return model;
        }

        public WebSite CreateSiteFromRemote(ApiCall call)
        {
            var domain = call.GetValue("FullDomain");
            if (string.IsNullOrEmpty(domain))
            {
                throw new Exception("Domain is empty");
            }

            var siteName = call.GetValue("SiteName");
            if (string.IsNullOrEmpty(siteName))
            {
                throw new Exception("siteName is empty");
            }

            var orgId = call.GetValue<Guid>("OrgId");
            var orgs = GlobalDb.Users.Organizations(call.Context.User.Id);
            if (orgs.All(a => a.Id != orgId))
            {
                throw new Exception("User does not have permission on this organization");
            }

            var existBinding = AppHost.BindingService.GetByFullDomain(domain);
            if (existBinding?.Any() ?? false)
            {
                throw new Exception("Domain is in use");
            }

            if (!AppHost.SiteService.CheckNameAvailable(siteName, orgId))
            {
                throw new Exception("Site name is occupied");
            }

            var newSite = Kooboo.Sites.Service.WebSiteService.AddNewSite(
                orgId,
                siteName,
                domain,
                call.Context.User.Id, true
            );

            if (string.IsNullOrWhiteSpace(newSite.BaseUrl))
            {
                newSite.BaseUrl = newSite.BaseUrl()?.Trim('/');
            }

            return newSite;
        }
        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public List<SyncSettingViewModel> List(ApiCall call)
        {
            List<SyncSettingViewModel> result = new List<SyncSettingViewModel>();

            var sitedb = call.WebSite.SiteDb();

            var list = sitedb.SyncSettings.All();

            foreach (var item in list)
            {
                SyncSettingViewModel model = new SyncSettingViewModel();
                model.Id = item.Id;
                model.RemoteServerUrl = item.RemoteServerUrl;
                model.RemoteSiteName = item.RemoteSiteName;
                model.ServerName = item.ServerName;
                model.LocalDifference = sitedb.SyncLog.PushItemsCount(item, call);
                model.Difference = model.LocalDifference;
                model.RemoteDifference = RemoteDiffer(item, sitedb, call);
                result.Add(model);
            }
            return result;
        }

        [Kooboo.Attributes.RequireParameters("id", "type")]
        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public Dictionary<string, IEnumerable<KeyValuePair<string, string>>> Options(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var type = call.GetValue("type");
            FacetSource source = FacetSource.PushOut;
            switch (type)
            {
                case "in":
                    source = FacetSource.PullIn;
                    break;
                case "out":
                    source = FacetSource.PushOut;
                    break;
                case "ignore":
                    source = FacetSource.Ignore;
                    break;
                default:
                    source = FacetSource.PushQueue;
                    break;
            }

            var facet = sitedb.SyncLog.ScanFacet(call.ObjectId, source, call);

            var storeNames = facet.StoreNames;
            var editTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                EditType.Add.ToString(),
                EditType.Update.ToString(),
                EditType.Delete.ToString()
            };

            var userMaps = new Dictionary<string, string>();

            foreach (var item in facet.UserIds)
            {
                if (!Kooboo.Data.Definition.BuiltInUser.IsBuiltInOrDefault(item))
                {
                    userMaps[item.ToString()] = Data.GlobalDb.Users.GetUserName(item);
                }
            }

            return new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>
            {
                [nameof(LogEntry.EditType)] = editTypes.ToArray()
                .Select(it => new KeyValuePair<string, string>(it, Hardcoded.GetValue(it, call.Context))),
                [nameof(LogEntry.StoreName)] = storeNames.ToArray()
                .Select(it => new KeyValuePair<string, string>(it, Hardcoded.GetValue(it, call.Context))).OrderBy(it => it.Key),
                [nameof(User)] = userMaps.ToList().OrderBy(it => it.Value)
            };
        }

        [Kooboo.Attributes.RequireModel(typeof(SyncSetting))]
        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public void Post(ApiCall call)
        {
            var siteId = call.GetGuidValue("SiteId");
            WebSite website = Data.Config.AppHost.SiteRepo.Get(siteId);

            if (website == null)
            {
                website = call.WebSite;
            }
            var setting = call.Context.Request.Model as SyncSetting;

            if (website != null)
            {

                if (!setting.RemoteServerUrl.ToLower().StartsWith("http"))
                {
                    setting.RemoteServerUrl = "http://" + setting.RemoteServerUrl;
                }

                if (setting.RemoteWebSiteId == default(Guid))
                {
                    string FullDomain = call.GetValue("FullDomain");
                    string SiteName = call.GetValue("SiteName");

                    if (!string.IsNullOrEmpty(FullDomain) && !string.IsNullOrEmpty(SiteName))
                    {
                        string url = setting.RemoteServerUrl + "/_api/publish/CreateSiteFromRemote";
                        Dictionary<string, string> para = new Dictionary<string, string>();
                        para.Add("FullDomain", FullDomain);
                        para.Add("SiteName", SiteName);
                        para.Add("OrgId", setting.OrgId.ToString());

                        var newSite = Lib.Helper.HttpHelper.Post<WebSite>(url, para,
                            new Dictionary<string, string> {
                               { "Authorization",$"bearer {UserProviderHelper.GetJtwTokentFromContext(call.Context)}" }
                            }, true);

                        if (newSite != null)
                        {
                            setting.RemoteSiteName = newSite.Name;
                            setting.RemoteWebSiteId = newSite.Id;
                        }
                    }
                }
                if (setting.RemoteWebSiteId != default(Guid))
                {
                    website.SiteDb().SyncSettings.AddOrUpdate(setting, call.Context.User.Id);
                }
            }
        }
        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public void Import(ApiCall call)
        {
            var setting = JsonHelper.Deserialize<SyncSetting>(call.Context.Request.Body);
            call.WebSite.SiteDb().SyncSettings.AddOrUpdate(setting, call.Context.User.Id);
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.DELETE)]
        public virtual bool Deletes(List<Guid> ids, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    sitedb.SyncSettings.Delete(item, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }


        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<ChangeItemViewModel> LocalChanges(Guid id, int PageNr, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var setting = sitedb.SyncSettings.Get(id);

            var all = sitedb.SyncLog.PushItems(setting, call).OrderBy(o => o.Id);

            int TotalCount = all.Count();

            int PageSize = 50;
            if (PageNr < 1)
            {
                PageNr = 1;
            }
            int skip = PageSize * (PageNr - 1);

            var dataList = all.Skip(skip).Take(PageSize);


            var renderContext = call.Context;

            List<ChangeItemViewModel> items = new List<ChangeItemViewModel>();

            foreach (var item in dataList)
            {
                var model = ChangeItemViewModel.FromLog(call.Context, item);
                if (model != default) items.Add(model);
            }

            PagedListViewModel<ChangeItemViewModel> result = new PagedListViewModel<ChangeItemViewModel>();
            result.List = items.ToList();
            result.TotalCount = TotalCount;
            result.PageNr = PageNr;
            result.PageSize = PageSize;

            return result;
        }


        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public PushFeedBack PushItem(Guid SettingId, ApiCall call)
        {
            var logId = call.GetValue<long>("LogId");
            return _Push(SettingId, logId, call, false);
        }

        public PushFeedBack ForcePush(Guid SettingId, long LogId, bool IgnoreCurrent, ApiCall call)
        {
            if (IgnoreCurrent)
            {
                var sitedb = call.Context.WebSite.SiteDb();
                var log = sitedb.Log.Get(LogId);
                Guid objectId = default;
                if (log == null)
                {
                    log = new LogEntry();
                }
                else
                {
                    objectId = ObjectContainer.GuidConverter.FromByte(log.KeyBytes);
                }

                sitedb.SyncLog.AddOut(SettingId, log.StoreName, log.TableColName, objectId, LogId, 0, call.Context.User.Id);
                return new PushFeedBack();
            }
            else
            {
                return _Push(SettingId, LogId, call, true);
            }
        }

        private PushFeedBack _Push(Guid SettingId, long LogId, ApiCall call, bool ForcePush)
        {
            var sitedb = call.WebSite.SiteDb();
            var setting = sitedb.SyncSettings.Get(SettingId);

            if (setting == null)
            {
                throw new Exception(Hardcoded.GetValue("Setting not found", call.Context));
            }

            var pushLog = GetPushLog(setting, sitedb, LogId, call);
            if (pushLog.IsFinish || pushLog.Log == null)
            {
                return new PushFeedBack() { IsFinish = true };
            }

            string ApiReceiveUrl = ForcePush ? "/_api/receiver/ForcePush" : "/_api/receiver/Push";
            string serverApiUrl = setting.RemoteServerUrl;
            if (!serverApiUrl.ToLower().StartsWith("http"))
            {
                serverApiUrl = "http://" + serverApiUrl;
            }
            serverApiUrl = UrlHelper.Combine(serverApiUrl, ApiReceiveUrl);

            var SyncItem = SyncService.Prepare(sitedb, pushLog.Log);
            SyncItem.SenderIdentifier = setting.UniqueId;
            SyncItem.RemoteSiteId = setting.RemoteWebSiteId;
            SyncItem.RemoteLastVersion = sitedb.SyncLog.GetRemoteBackVersion(setting.Id, SyncItem.ObjectId);

            var response = SyncService.SendToRemote(sitedb, SyncItem, serverApiUrl, setting.AccessToken ?? UserProviderHelper.GetJtwTokentFromContext(call.Context));

            if (response.HasConflict == false)
            {
                sitedb.SyncLog.AddOut(setting.Id, SyncItem.StoreName, SyncItem.TableName, SyncItem.ObjectId, SyncItem.SenderVersion, response.ReceiverVersion, SyncItem.UserId);

                return new PushFeedBack() { IsFinish = false, SiteLogId = pushLog.SiteLogId };
            }
            else
            {
                var result = new PushFeedBack() { HasConflict = true, IsFinish = false, RemoteBody = response.DisplayBody, RemoteVersion = response.ReceiverVersion, RemoteUserName = response.UserName, RemoteTime = response.LastModified, IsImage = response.IsImage };

                result.IsImage = response.IsImage;

                result.LocalBody = SyncService.GetLogSummaryBody(sitedb, pushLog.Log);
                result.SiteLogId = pushLog.SiteLogId;

                result.LocalTime = pushLog.Log.UpdateTime;
                result.LocalUserName = Data.GlobalDb.Users.GetUserName(pushLog.Log.UserId);

                return result;
            }
        }

        private PushLog GetPushLog(SyncSetting setting, SiteDb sitedb, long LogId, ApiCall call)
        {
            var result = new PushLog();


            if (LogId > 0)
            {
                result.SiteLogId = LogId;
                result.Log = sitedb.Log.Get(LogId);
            }
            else
            {
                var logs = sitedb.SyncLog.PushItems(setting, call, 3);

                if (logs == null || !logs.Any())
                {
                    return new PushLog() { IsFinish = true };
                }
                else
                {
                    result.SiteLogId = logs[0].Id;
                    result.Log = logs.FirstOrDefault();
                    result.SiteLogId = result.Log != null ? result.Log.Id : 0;
                }
            }
            return result;
        }


        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public void IgnoreLog(Guid SettingId, List<long> logs, ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            foreach (var item in logs)
            {
                siteDb.SyncLog.IgnoreLog(SettingId, item);
            }
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<ChangeItemViewModel> IgnoreList(Guid SettingId, int PageNr, int PageSize, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var filter = new LogFilter(call);
            var ignoreList = sitedb.SyncLog.ListIgnoreObj(SettingId).Where(w =>
            {
                var logEntry = sitedb.Log.Get(w.LogId);
                if (logEntry == default) return false;
                return filter.Match(logEntry);
            }).ToList();


            PagedListViewModel<ChangeItemViewModel> result = new PagedListViewModel<ChangeItemViewModel>();
            result.TotalCount = ignoreList.Count;

            if (PageSize < 1)
            {
                PageSize = 20;
            }

            if (PageNr < 1)
            {
                PageNr = 1;
            }
            int skip = PageSize * (PageNr - 1);

            ignoreList = ignoreList.OrderByDescending(o => o.lastModified).Skip(skip).Take(PageSize).ToList();
            result.PageNr = PageNr;
            result.PageSize = PageSize;
            result.List = new List<ChangeItemViewModel>();

            foreach (var item in ignoreList)
            {
                var logEntry = sitedb.Log.Get(item.LogId);
                var model = ChangeItemViewModel.FromLog(call.Context, logEntry);
                result.List.Add(model);

            }

            return result;

        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public void UnIgnore(Guid SettingId, List<Guid> ObjectIds, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            foreach (var item in ObjectIds)
            {
                sitedb.SyncLog.UnIgnore(SettingId, item);
            }
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public PullFeedBack Pull(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var setting = sitedb.SyncSettings.Get(id);
            if (setting == null)
            {
                throw new Exception(Hardcoded.GetValue("Setting not found", call.Context));
            }

            long currentRemoteVersion = call.GetValue<long>("senderVersion");

            var lastIn = sitedb.SyncLog.GetRemoteVersion(setting.Id);

            if (currentRemoteVersion < lastIn)
            {
                currentRemoteVersion = lastIn;
            }
            return _pull(setting, currentRemoteVersion, false, call);
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public PullFeedBack ForcePull(Guid id, long currentSenderVersion, bool IgnoreCurrent, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var setting = sitedb.SyncSettings.Get(id);
            if (setting == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Setting not found", call.Context));
            }

            //when IgnoreCurrent = use local.
            if (IgnoreCurrent)
            {
                var localLogId = call.GetValue<long>("localLogId");
                var senderVersion = call.GetValue<long>("senderVersion");

                var log = sitedb.Log.Get(localLogId);
                if (log != null)
                {
                    var objectId = ObjectContainer.GuidConverter.FromByte(log.KeyBytes);
                    sitedb.SyncLog.AddIn(setting.Id, log.StoreName, log.TableColName, objectId, 0, senderVersion, log.UserId);
                }
                return new PullFeedBack() { IsFinish = false, SenderVersion = senderVersion };
            }
            else
            {
                return _pull(setting, currentSenderVersion, true, call);
            }


        }


        private PullFeedBack _pull(SyncSetting setting, long currentRemoteVersion, bool ForcePull, ApiCall call)
        {

            var syncObject = _getRemote(setting, currentRemoteVersion, call);

            if (syncObject != null)
            {
                var sitedb = call.Context.WebSite.SiteDb();

                if (!ForcePull)
                {
                    var conflictCheck = Kooboo.Sites.Sync.ConflictService.instance.CheckPullFrom(syncObject, sitedb, setting);

                    if (conflictCheck.HasConflict)
                    {

                        PullFeedBack feedback = new PullFeedBack();
                        feedback.IsFinish = false;
                        feedback.SenderVersion = syncObject.SenderVersion;
                        feedback.CurrentSenderVersion = currentRemoteVersion;

                        feedback.HasConflict = true;

                        feedback.RemoteUserName = Kooboo.Data.GlobalDb.Users.GetUserName(syncObject.UserId);

                        if (conflictCheck.log != null)
                        {
                            feedback.LocalUserName = Kooboo.Data.GlobalDb.Users.GetUserName(conflictCheck.log.UserId);
                            feedback.LocalTime = conflictCheck.log.UpdateTime;
                            feedback.LocalLogId = conflictCheck.log.Id;
                        }

                        if (conflictCheck.IsTable)
                        {
                            feedback.LocalBody = Kooboo.Sites.Service.ObjectService.GetSummaryText(conflictCheck.TableData);

                            var data = SyncObjectConvertor.FromTableSyncObject(syncObject);

                            feedback.RemoteBody = ObjectService.GetSummaryText(data);

                        }
                        else
                        {

                            var remoteSiteObject = SyncObjectConvertor.FromSyncObject(syncObject);

                            if (remoteSiteObject != null)
                            {
                                feedback.RemoteTime = remoteSiteObject.LastModified;
                            }


                            if (conflictCheck.SiteObject is Kooboo.Sites.Models.Image)
                            {
                                feedback.IsImage = true;

                                var localImage = conflictCheck.SiteObject as Kooboo.Sites.Models.Image;
                                if (localImage != null)
                                {
                                    feedback.LocalBody = Convert.ToBase64String(localImage.ContentBytes);
                                }

                                if (remoteSiteObject != null)
                                {
                                    var remoteImage = remoteSiteObject as Kooboo.Sites.Models.Image;

                                    if (remoteImage != null)
                                    {
                                        feedback.RemoteBody = Convert.ToBase64String(remoteImage.ContentBytes);
                                    }
                                }

                            }
                            else
                            {
                                feedback.LocalBody = Kooboo.Sites.Service.ObjectService.GetSummaryText(conflictCheck.SiteObject);
                                if (remoteSiteObject != null)
                                {
                                    feedback.RemoteBody = ObjectService.GetSummaryText(remoteSiteObject);
                                }

                            }

                        }
                        return feedback;

                    }
                    else
                    {
                        SyncService.Receive(sitedb, syncObject, setting);

                        return new PullFeedBack() { IsFinish = false, SenderVersion = syncObject.SenderVersion };
                    }

                }

                else
                {
                    SyncService.Receive(sitedb, syncObject, setting);

                    return new PullFeedBack() { IsFinish = false, SenderVersion = syncObject.SenderVersion };
                }

            }
            else
            {
                return new PullFeedBack() { IsFinish = true };
            }

        }


        private SyncObject _getRemote(SyncSetting setting, long LastId, ApiCall call)
        {
            string ApiSendToLocalUrl = "/_api/publish/SendToClient";
            string serverApiURL = setting.RemoteServerUrl;

            if (!serverApiURL.ToLower().StartsWith("http"))
            {
                serverApiURL = "http://" + serverApiURL;
            }
            serverApiURL = UrlHelper.Combine(serverApiURL, ApiSendToLocalUrl);

            PullRequestParameter para = new()
            {
                LastId = LastId,
                SiteId = setting.RemoteWebSiteId,
                SenderUniqueId = setting.UniqueId,
                IgnoreStores = setting.IgnoreInStoreNames
            };

            string paraJson = System.Text.Json.JsonSerializer.Serialize(para);

            return Kooboo.Lib.Helper.HttpHelper.Post<SyncObject>(serverApiURL, paraJson,
               new Dictionary<string, string>
               {
                      { "Authorization",$"bearer {setting.AccessToken??UserProviderHelper.GetJtwTokentFromContext(call.Context)}"}
               });
        }

        public SyncObject SendToClient(PullRequestParameter model, ApiCall call)
        {
            return PullRequest.PullNext(call.WebSite.SiteDb(), model);
        }

        private int RemoteDiffer(SyncSetting setting, SiteDb sitedb, ApiCall call)
        {
            var lastIn = sitedb.SyncLog.GetRemoteVersion(setting.Id);

            string ApiSendToLocalUrl = "/_api/publish/ClientPullDiffer";
            string serverApiURL = setting.RemoteServerUrl;

            if (!serverApiURL.ToLower().StartsWith("http"))
            {
                serverApiURL = "http://" + serverApiURL;
            }
            serverApiURL = UrlHelper.Combine(serverApiURL, ApiSendToLocalUrl);

            PullRequestParameter para = new()
            {
                LastId = lastIn,
                SiteId = setting.RemoteWebSiteId,
                SenderUniqueId = setting.UniqueId,
                IgnoreStores = setting.IgnoreInStoreNames
            };

            string paraJson = System.Text.Json.JsonSerializer.Serialize(para);

            try
            {
                return Kooboo.Lib.Helper.HttpHelper.Post<int>(serverApiURL, paraJson,
              new Dictionary<string, string>
              {
                      { "Authorization",$"bearer {setting.AccessToken?? UserProviderHelper.GetJtwTokentFromContext(call.Context)}"}
              });
            }
            catch (Exception)
            {

            }
            return 0;
        }


        public int ClientPullDiffer(PullRequestParameter para, ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            PullDiffer differ = new PullDiffer(siteDb, para);

            return differ.Count();
        }


        public long VersionNumber(ApiCall call)
        {
            return call.WebSite.SiteDb().Log.Store.LastKey;
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<ChangeItemViewModel> OutItem(Guid SyncSettingId, ApiCall call)
        {
            return GetLogItem(call, SyncSettingId, false);
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<ChangeItemViewModel> InItem(Guid SyncSettingId, ApiCall call)
        {
            return GetLogItem(call, SyncSettingId, true);
        }

        private PagedListViewModel<ChangeItemViewModel> GetLogItem(ApiCall call, Guid SyncSettingId, bool InItem)
        {
            int pageSize = ApiHelper.GetPageSize(call);
            int pageNr = ApiHelper.GetPageNr(call);

            var (dataList, total) = getSyncLog(call, SyncSettingId, pageSize, pageNr, InItem);

            var sitedb = call.Context.WebSite.SiteDb();

            PagedListViewModel<ChangeItemViewModel> result = new PagedListViewModel<ChangeItemViewModel>();

            result.TotalCount = (int)total;
            result.PageSize = pageSize;
            result.PageNr = pageNr;
            result.List = new List<ChangeItemViewModel>();

            foreach (var item in dataList.OrderByDescending(o => o.Id))
            {
                var log = sitedb.Log.Get(item.LocalVersion);

                if (log != null)
                {
                    var model = ChangeItemViewModel.FromLog(call.Context, log);
                    if (model != default) result.List.Add(model);
                }
            }

            return result;

        }

        private (List<SyncLog>, long) getSyncLog(ApiCall call, Guid SettingId, int PageSize, int PageNr, bool IsIn)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            if (IsIn)
            {
                return siteDb.SyncLog.GetInRecords(SettingId, PageSize, PageNr, call);
            }
            else
            {
                return siteDb.SyncLog.GetOutRecords(SettingId, PageSize, PageNr, call);
            }
        }

        public void SetProgress(Guid SyncSettingId, long LocalProgress, long RemoteProgress, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var setting = _getSetting(sitedb, SyncSettingId);

            sitedb.SyncLog.SetLastReadVersion(setting.Id, LocalProgress);

            sitedb.SyncLog.SetOutProgress(SyncSettingId, RemoteProgress);
        }

        public SyncProgress GetProgress(Guid SyncSettingId, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var setting = _getSetting(sitedb, SyncSettingId);

            // call to set the last read.
            sitedb.SyncLog.PushItems(setting, call, 3);

            var localProgress = sitedb.SyncLog.GetLastReadVersion(SyncSettingId);
            var localMax = sitedb.Log.Store.LastKey;

            var remoteProgress = sitedb.SyncLog.GetRemoteVersion(SyncSettingId);

            string ApiSendToLocalUrl = "/_api/publish/LogLastKey";
            string serverApiURL = setting.RemoteServerUrl;

            if (!serverApiURL.ToLower().StartsWith("http"))
            {
                serverApiURL = "http://" + serverApiURL;
            }
            serverApiURL = UrlHelper.Combine(serverApiURL, ApiSendToLocalUrl);

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("remoteSiteId", setting.RemoteWebSiteId.ToString());

            long remoteLastVersion = 0;

            try
            {
                remoteLastVersion = Kooboo.Lib.Helper.HttpHelper.Post<long>(serverApiURL, para,
                new Dictionary<string, string>
                {
                      { "Authorization",$"bearer {UserProviderHelper.GetJtwTokentFromContext(call.Context)}"}
                });
            }
            catch (Exception)
            {

            }


            return new SyncProgress() { LocalLastId = localMax, LocalProgress = localProgress, RemoteLastId = remoteLastVersion, RemoteProgress = remoteProgress };
        }


        public long LogLastKey(Guid remoteSiteId, ApiCall call)
        {
            var website = AppHost.SiteRepo.Get(remoteSiteId);
            if (website != null)
            {
                return website.SiteDb().Log.Store.LastKey;
            }
            return 0;
        }

        public List<OrgServerHost> OrgServerList(Guid orgId, ApiCall call)
        {
            return UserPublishService.OrgServerList(orgId, call.Context);
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public Difference[] GetSettingDifferences(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var setting = _getSetting(sitedb, id);
            return SettingsSync.GetDifferences(call.WebSite.Id, setting, UserProviderHelper.GetJtwTokentFromContext(call.Context));
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public Sites.Sync.Settings.SettingsItem[] GetSettings(ApiCall call)
        {
            return SettingsSync.GetSettings(call.WebSite.Id);
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public void PullSettings(ApiCall call)
        {
            var settings = JsonHelper.Deserialize<Sites.Sync.Settings.SettingsItem[]>(call.Context.Request.Body);
            SettingsSync.Pull(call.WebSite.Id, settings);
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public void PushSettings(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var setting = _getSetting(sitedb, id);
            var settings = JsonHelper.Deserialize<SettingsItem[]>(call.Context.Request.Body);
            SettingsSync.Push(setting, UserProviderHelper.GetJtwTokentFromContext(call.Context), settings);
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.VIEW)]
        public SyncSetting GetSyncSetting(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var setting = _getSetting(sitedb, id);
            return setting;
        }

        [Permission(Feature.SYNC, Action = Data.Permission.Action.EDIT)]
        public void UpdateSyncSetting(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var setting = _getSetting(sitedb, id);
            var newSetting = JsonHelper.Deserialize<SyncSetting>(call.Context.Request.Body);
            setting.IgnoreInStoreNames = newSetting.IgnoreInStoreNames;
            setting.IgnoreOutStoreNames = newSetting.IgnoreOutStoreNames;
            sitedb.SyncSettings.AddOrUpdate(setting);


        }

        readonly string[] _ignoreStores = new[] { "PaymentCustomer", "PaymentRequest", "TableSchemaMapping", "ViewDataMethod", "ScriptModule", "DataMethodSetting" };
        public KeyValuePair<string, string>[] StoreNames(ApiCall call)
        {
            var types = SiteRepositoryContainer.CoreObjectTypes;
            var result = new List<KeyValuePair<string, string>>();

            foreach (var item in types)
            {
                var name = ConstTypeContainer.GetName(ConstTypeContainer.GetConstType(item));
                if (_ignoreStores.Contains(name)) continue;
                result.Add(new KeyValuePair<string, string>(name, Hardcoded.GetValue(name, call.Context)));
            }
            result.Add(new KeyValuePair<string, string>("Storage", Hardcoded.GetValue("Table", call.Context)));
            return result.OrderBy(o => o.Value).ToArray();
        }

        private SyncSetting _getSetting(SiteDb sitedb, Guid SyncSettingId)
        {
            if (sitedb != null)
            {
                var setting = sitedb.SyncSettings.Get(SyncSettingId);
                if (setting == null)
                {
                    throw new Exception("Setting Not Found");
                }
                return setting;
            }
            return null;
        }
    }
}

