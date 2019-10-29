//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Utilities;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using Kooboo.Sites.Sync;
using Kooboo.Sites.TaskQueue.Model;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var orgid = call.GetValue<Guid>("OrganizationId");
            if (orgid == default(Guid))
            {
                orgid = user.CurrentOrgId;
            }

            var sites = WebSiteService.RemoteListByUser(user.Id, orgid);

            List<SimpleSiteItemViewModel> result = new List<SimpleSiteItemViewModel>();

            foreach (var item in sites)
            {
                var lastversion = item.SiteDb().Log.Store.LastKey;

                result.Add(new SimpleSiteItemViewModel { Id = item.Id, Name = item.Name, LastVersion = lastversion });
            }

            return result;
        }

        public List<SimpleSiteItemViewModel> RemoteSiteList(string remoteurl, ApiCall call)
        {
            if (string.IsNullOrEmpty(remoteurl))
            {
                return null;
            }

            var user = call.Context.User;
            Guid OrganizationId = Service.UserService.GuessOrgId(user, remoteurl);
            string username = user.UserName;
            string password = Data.Service.UserLoginService.GetUserPassword(user);


            if (!remoteurl.ToLower().StartsWith("http"))
            {
                remoteurl = "http://" + remoteurl;
            }
            remoteurl = remoteurl.Replace("\\", "/");
            if (!remoteurl.EndsWith("/"))
            {
                remoteurl = remoteurl + "/";
            }

            remoteurl = remoteurl + "_api/publish/sitelist?OrganizationId=" + OrganizationId.ToString();

            List<SimpleSiteItemViewModel> model = Lib.Helper.HttpHelper.Get<List<SimpleSiteItemViewModel>>(remoteurl, null, username, password);

            return model;
        }


        public List<SyncItemViewModel> List(ApiCall call)
        {
            List<SyncItemViewModel> result = new List<SyncItemViewModel>();

            var sitedb = call.WebSite.SiteDb();

            var list = sitedb.SyncSettings.All();

            foreach (var item in list)
            {
                SyncItemViewModel model = new SyncItemViewModel();
                model.Id = item.Id;
                model.RemoteServerUrl = item.RemoteServerUrl;
                model.RemoteSiteName = item.RemoteSiteName;
                model.Difference = sitedb.Synchronization.QueueCount(item.Id);
                result.Add(model);
            }
            return result;
        }

        [Kooboo.Attributes.RequireModel(typeof(SyncSetting))]
        public void Post(ApiCall call)
        {
            var siteid = call.GetGuidValue("SiteId");
            Data.Models.WebSite website = Data.GlobalDb.WebSites.Get(siteid);

            if (website == null)
            {
                website = call.WebSite;
            }
            var setting = call.Context.Request.Model as SyncSetting;

            if (website != null)
            {
                ///TODO: if remotesiteId == default(guid), call to create remote site id... 
                // url... /_api/site/create, FullDomain, SiteName.... 

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
                        string url = setting.RemoteServerUrl + "/_api/site/create";
                        Dictionary<string, string> para = new Dictionary<string, string>();
                        para.Add("FullDomain", FullDomain);
                        para.Add("SiteName", SiteName);

                        var newsite = Lib.Helper.HttpHelper.Get<WebSite>(url, para, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());

                        if (newsite != null)
                        {
                            setting.RemoteSiteName = newsite.Name;
                            setting.RemoteWebSiteId = newsite.Id;
                        }
                    }
                }
                if (setting.RemoteWebSiteId != default(Guid))
                {
                    website.SiteDb().SyncSettings.AddOrUpdate(setting, call.Context.User.Id);
                }
            }
        }

        public virtual bool Deletes(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            List<Guid> ids = new List<Guid>();

            try
            {
                ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);
            }
            catch (Exception)
            {
                //throw;
            }

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

        [Kooboo.Attributes.RequireParameters("id")]
        public List<PushItemViewModel> PushItems(ApiCall call)
        {
            List<PushItemViewModel> result = new List<PushItemViewModel>();

            var sitedb = call.WebSite.SiteDb();
            var kdb = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);

            foreach (var item in sitedb.Synchronization.GetPushItems(call.ObjectId))
            {
                ChangeType changetype = ChangeType.Add;
                if (item.EditType == IndexedDB.EditType.Delete)
                {
                    changetype = ChangeType.Delete;
                }
                else if (item.EditType == IndexedDB.EditType.Update)
                {
                    changetype = ChangeType.Update;
                }

                if (item.IsTable)
                {

                    var table = Data.DB.GetTable(kdb, item.TableName);
                    if (table != null)
                    {
                        var logdata = table.GetLogData(item);
                        PushItemViewModel viewmodel = new PushItemViewModel
                        {
                            Id = item.Id,
                            ObjectType = Kooboo.Data.Language.Hardcoded.GetValue("Table", call.Context),
                            Name = LogService.GetTableDisplayName(sitedb, item, call.Context),
                            // KoobooType = siteojbect.ConstType,
                            ChangeType = changetype,
                            LastModified = item.UpdateTime,
                            LogId = item.Id
                        };
                        result.Add(viewmodel);
                    }
                }
                else
                {

                    var siteojbect = ObjectService.GetSiteObject(sitedb, item);
                    if (siteojbect == null)
                    {
                        continue;
                    }

                    PushItemViewModel viewmodel = new PushItemViewModel
                    {
                        Id = item.Id,
                        ObjectType = ConstTypeService.GetModelType(siteojbect.ConstType).Name,
                        Name = LogService.GetLogDisplayName(sitedb, item),
                        KoobooType = siteojbect.ConstType,
                        ChangeType = changetype,
                        LastModified = siteojbect.LastModified,
                        LogId = item.Id
                    };

                    viewmodel.Size = CalculateUtility.GetSizeString(ObjectService.GetSize(siteojbect));

                    if (siteojbect.ConstType == ConstObjectType.Image)
                    {
                        viewmodel.Thumbnail = ThumbnailService.GenerateThumbnailUrl(siteojbect.Id, 50, 50, call.WebSite.Id);
                    }

                    result.Add(viewmodel);
                }

            }

            return result.OrderBy(o=>o.LogId).ToList();
        }

        [Kooboo.Attributes.RequireParameters("id", "logids")]
        public void Push(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var setting = sitedb.SyncSettings.Get(call.ObjectId);
            if (setting == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Setting not found", call.Context));
            }

            verifysite(setting.RemoteServerUrl, setting.RemoteWebSiteId, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());

            string ApiReceiveUrl = "/_api/receiver/push";
            string serveapiurl = setting.RemoteServerUrl;
            if (!serveapiurl.ToLower().StartsWith("http"))
            {
                serveapiurl = "http://" + serveapiurl;
            }
            serveapiurl = Kooboo.Lib.Helper.UrlHelper.Combine(serveapiurl, ApiReceiveUrl);

            string json = call.GetValue("logids");

            List<long> ids = new List<long>();

            bool DirectSubmit = false;

            try
            {
                ids = Lib.Helper.JsonHelper.Deserialize<List<long>>(json);
                if (ids.Count() < 5)
                {
                    DirectSubmit = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            bool hasqueue = false;
            Kooboo.Sites.TaskQueue.TaskExecutor.PostSyncObjectTask executor = new Sites.TaskQueue.TaskExecutor.PostSyncObjectTask();

            string username = call.Context.User.UserName;
            string password = Data.Service.UserLoginService.GetUserPassword(call.Context.User);


            foreach (var item in ids)
            {
                var eachsync = SyncService.Prepare(sitedb, item);
                PostSyncObject postobject = new PostSyncObject();
                postobject.SyncObject = eachsync;
                postobject.RemoteSiteId = setting.RemoteWebSiteId;
                postobject.RemoteUrl = serveapiurl;
                postobject.UserName = username;
                postobject.Password = password;
                postobject.RemoteSiteId = setting.RemoteWebSiteId;

                if (DirectSubmit)
                {
                    if (!executor.Execute(sitedb, Lib.Helper.JsonHelper.Serialize(postobject)))
                    {
                        Sites.TaskQueue.QueueManager.Add(postobject, call.WebSite.Id);
                        hasqueue = true;
                    }
                }
                else
                {
                    Sites.TaskQueue.QueueManager.Add(postobject, call.WebSite.Id);
                    hasqueue = true;
                }

                sitedb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = setting.Id, StoreName = eachsync.StoreName, ObjectId = eachsync.ObjectId, In = false, Version = item });
            }

            if (hasqueue)
            {
                System.Threading.Tasks.Task.Run(() => Sites.TaskQueue.QueueManager.Execute(call.WebSite.Id));
            }
        }
         
        public PullResult Pull(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var setting = sitedb.SyncSettings.Get(id);
            if (setting == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Setting not found", call.Context));
            }
            long currentRemoteVersion = call.GetValue<long>("senderVersion");
            if (currentRemoteVersion <= 0)
            {
                currentRemoteVersion = sitedb.Synchronization.GetLastIn(setting.Id);
            }

            string ApiSendToLocalUrl = "/_api/publish/SendToClient";
            string serveapiurl = setting.RemoteServerUrl;

            verifysite(setting.RemoteServerUrl, setting.RemoteWebSiteId, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());

            if (!serveapiurl.ToLower().StartsWith("http"))
            {
                serveapiurl = "http://" + serveapiurl;
            }
            serveapiurl = Kooboo.Lib.Helper.UrlHelper.Combine(serveapiurl, ApiSendToLocalUrl);

            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("SiteId", setting.RemoteWebSiteId.ToString());
            query.Add("LastId", currentRemoteVersion.ToString());

            var syncobject = Kooboo.Lib.Helper.HttpHelper.Get<SyncObject>(serveapiurl, query, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());

            PullResult result = new PullResult();

            if (syncobject != null)
            {
                SyncService.Receive(sitedb, syncobject, setting);
                result.IsFinish = false;
                result.SenderVersion = syncobject.SenderVersion;
            }
            else
            {
                result.IsFinish = true;
            }
            return result;
        }


        private void verifysite(string removeservrerurl, Guid remotesiteid, string username, string password)
        {
            string ApiSendToLocalUrl = "/_api/publish/CheckSite?SiteId=" + remotesiteid.ToString();

            if (!removeservrerurl.ToLower().StartsWith("http"))
            {
                removeservrerurl = "http://" + removeservrerurl;
            }
            string fullurl = Kooboo.Lib.Helper.UrlHelper.Combine(removeservrerurl, ApiSendToLocalUrl);

            try
            {
                var model = Lib.Helper.HttpHelper.Get<CheckResponse>(fullurl, null, username, password);
                if (model.CheckResult == false)
                {
                    throw new Exception(model.Message);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CheckResponse CheckSite(Guid SiteId, ApiCall call)
        {
            var site = Kooboo.Data.GlobalDb.WebSites.Get(SiteId);

            var res = new CheckResponse();

            if (site == null)
            {
                res.CheckResult = false;
                res.Message = Data.Language.Hardcoded.GetValue("Website not found", call.Context);
                res.Translated = true;
            }
            else
            {
                res.CheckResult = true;
            }
            return res;
        }

        // [Kooboo.Attributes.RequireParameters("SiteId", "LastId")]
        public SyncObject SendToClient(long LastId, Guid SiteId,  ApiCall call)
        {
              return PullRequest.PullNext(call.WebSite.SiteDb(), LastId);
          
        }

        //[Kooboo.Attributes.RequireParameters("logid")]
        public long VersionNumber(ApiCall call)
        {
            return call.WebSite.SiteDb().Log.Store.LastKey;
        }

        [Kooboo.Attributes.RequireParameters("SyncSettingId")]
        public PagedListViewModel<SyncLogItemViewModel> OutItem(ApiCall call)
        {
            return GetLogItem(call, false);
        }

        [Kooboo.Attributes.RequireParameters("SyncSettingId")]
        public PagedListViewModel<SyncLogItemViewModel> InItem(ApiCall call)
        {
            return GetLogItem(call, true);
        }

        private PagedListViewModel<SyncLogItemViewModel> GetLogItem(ApiCall call, bool initem)
        {
            var sitedb = call.WebSite.SiteDb();

            int pagesize = ApiHelper.GetPageSize(call);
            int pagenr = ApiHelper.GetPageNr(call);

            PagedListViewModel<SyncLogItemViewModel> model = new PagedListViewModel<SyncLogItemViewModel>();
            model.PageNr = pagenr;
            model.PageSize = pagesize;

            List<SyncLogItemViewModel> result = new List<SyncLogItemViewModel>();

            Guid settingid = call.GetGuidValue("SyncSettingId");

            var items = sitedb.Synchronization.Query.Where(o => o.SyncSettingId == settingid && o.In == initem).SelectAll().OrderByDescending(o => o.LastModifyTick);

            model.TotalCount = items.Count();
            model.TotalPages = ApiHelper.GetPageCount(model.TotalCount, model.PageSize);

            foreach (var item in items.Skip(model.PageNr * model.PageSize - model.PageSize).Take(model.PageSize))
            {
                var log = sitedb.Log.Get(item.Version);
                if (log != null)
                {
                    ChangeType changetype;
                    if (log.EditType == IndexedDB.EditType.Add)
                    {
                        changetype = ChangeType.Add;
                    }
                    else if (log.EditType == IndexedDB.EditType.Update)
                    { changetype = ChangeType.Update; }
                    else
                    {
                        changetype = ChangeType.Delete;
                    }

                    if (log.IsTable)
                    {
                        var kdb = Kooboo.Data.DB.GetKDatabase(sitedb.WebSite);
                        var table = Data.DB.GetTable(kdb, log.TableName);

                        if (table != null)
                        {
                            var logdata = table.GetLogData(log);

                            string size = null;
                            if (logdata != null)
                            {
                                var json = Lib.Helper.JsonHelper.Serialize(logdata);
                                size = CalculateUtility.GetSizeString(json.Length);
                            }

                            var name = Kooboo.Sites.Service.LogService.GetTableDisplayName(sitedb, log, call.Context, logdata);

                            SyncLogItemViewModel logitem = new SyncLogItemViewModel();
                            logitem.Name = name;

                            logitem.ObjectType = Data.Language.Hardcoded.GetValue("Table", call.Context);
                            logitem.LastModified = log.UpdateTime;
                            logitem.LogId = log.Id;
                            logitem.ChangeType = changetype;

                            result.Add(logitem);
                        }
                    }

                    else
                    {
                        var repo = sitedb.GetRepository(item.StoreName);

                        if (repo != null)
                        {
                            var siteobject = repo.GetByLog(log);

                            SyncLogItemViewModel logitem = new SyncLogItemViewModel();
                            var info = ObjectService.GetObjectInfo(sitedb, siteobject as ISiteObject);
                            logitem.Name = info.Name;
                            logitem.Size = Lib.Utilities.CalculateUtility.GetSizeString(info.Size);
                            logitem.ObjectType = repo.StoreName;
                            logitem.LastModified = log.UpdateTime;
                            logitem.LogId = log.Id;
                            logitem.ChangeType = changetype;

                            result.Add(logitem);
                        }
                    }
                }
            }
            model.List = result;
            return model;
        }
    }
}

