//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Sync.Cluster
{
    public static class ClusterService
    {
        private static string ClusterUpdateUrl = "/api/admin/cluster/update";
        private static string ClusterSyncSettingUrl = "api/admin/cluster/setting";

        private static object _locker = new object();

        private static Dictionary<Guid, List<Guid>> ClusterTo = new Dictionary<Guid, List<Guid>>();

        public static List<Guid> GetClusterTo(Data.Models.WebSite webSite)
        {
            lock (_locker)
            {
                if (!ClusterTo.ContainsKey(webSite.Id))
                {
                    var tos = CalculateClusterTo(webSite);
                    ClusterTo[webSite.Id] = tos;
                }
                return ClusterTo[webSite.Id];
            }
        }

        internal static List<Guid> CalculateClusterTo(Data.Models.WebSite webSite)
        {
            HashSet<Guid> result = new HashSet<Guid>();

            var store = Stores.ClusterNodes(webSite.SiteDb());

            var allitems = store.Filter.SelectAll().OrderBy(o => o.ServerUrl).ToList();

            var count = allitems.Count();
            if (count == 0)
            {
                return new List<Guid>();
            }
            int currentposition = -1;

            for (int i = 0; i < count; i++)
            {
                if (allitems[i].ServerWebSiteId == webSite.Id)
                {
                    currentposition = i;
                }
            }

            int one = currentposition + 1;
            int two = one + 2;
            int three = two + 4;

            one = CorrectIndex(one, count);
            two = CorrectIndex(two, count);
            three = CorrectIndex(three, count);

            result.Add(allitems[one].Id);
            result.Add(allitems[two].Id);
            result.Add(allitems[three].Id);
            if (currentposition > -1)
            {
                result.Remove(allitems[currentposition].Id);
            }

            return result.ToList();
        }

        private static int CorrectIndex(int index, int totalcount)
        {
            if (index > totalcount - 1)
            {
                return CorrectIndex(index - totalcount, totalcount);
            }
            return index;
        }

        public static void EnableLocalSite(Guid localWebSiteId, string localUrl, string localUserName, string localPassword)
        {
            lock (_locker)
            {
                var website = Data.GlobalDb.WebSites.Get(localWebSiteId);
                if (website != null)
                {
                    if (!website.EnableCluster)
                    {
                        website.EnableCluster = true;
                        Data.GlobalDb.WebSites.AddOrUpdate(website);
                        website = Data.GlobalDb.WebSites.Get(localWebSiteId);
                    }

                    var store = Stores.ClusterNodes(website.SiteDb());

                    if (string.IsNullOrEmpty(localUrl))
                    {
                        localUrl = GetLocalServerUrl(localWebSiteId);
                    }

                    AddOrUpdate(store, localUrl, localUserName, localPassword, localWebSiteId, true);

                    ClusterTo.Remove(localWebSiteId);
                }
            }
        }

        /// <summary>
        /// Add remote server and then need to PostToCheckIn.
        /// </summary>
        /// <param name="localWebSiteId"></param>
        /// <param name="remoteServerUrl"></param>
        /// <param name="remoteServerUserName"></param>
        /// <param name="remoteServerPassword"></param>
        /// <param name="remoteWebSiteId"></param>
        public static void AddRemoteServer(Guid localWebSiteId, string remoteServerUrl, string remoteServerUserName, string remoteServerPassword, Guid remoteWebSiteId)
        {
            lock (_locker)
            {
                var website = Data.GlobalDb.WebSites.Get(localWebSiteId);
                if (website != null)
                {
                    if (!website.EnableCluster)
                    {
                        website.EnableCluster = true;
                        Data.GlobalDb.WebSites.AddOrUpdate(website);
                        website = Data.GlobalDb.WebSites.Get(localWebSiteId);
                    }

                    var store = Stores.ClusterNodes(website.SiteDb());

                    AddOrUpdate(store, remoteServerUrl, remoteServerUserName, remoteServerPassword, remoteWebSiteId);

                    ClusterTo.Remove(localWebSiteId);
                }
            }
        }

        internal static ClusterNode AddOrUpdate(ObjectStore<Guid, ClusterNode> store, string serverUrl, string userName, string password, Guid targetWebSiteId, bool isLocal = false)
        {
            ClusterNode item = new ClusterNode
            {
                ServerUrl = serverUrl,
                UserName = userName,
                Password = password,
                ServerWebSiteId = targetWebSiteId,
                IsLocal = isLocal
            };

            var old = store.get(item.Id);

            if (old == null)
            {
                store.add(item.Id, item);
                return item;
            }
            else
            {
                if (old.GetHashCode() != item.GetHashCode())
                {
                    store.update(item.Id, item);
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool CheckInByRemote(Guid localWebSiteId, ClusterNode remoteNode)
        {
            lock (_locker)
            {
                var website = Data.GlobalDb.WebSites.Get(localWebSiteId);
                if (website != null)
                {
                    if (!website.EnableCluster)
                    {
                        return false;
                    }

                    var store = Stores.ClusterNodes(website.SiteDb());

                    if (store == null)
                    {
                        return false;
                    }

                    var node = AddOrUpdate(store, remoteNode.ServerUrl, remoteNode.UserName, remoteNode.Password, remoteNode.ServerWebSiteId);
                    if (node != null)
                    {
                        SyncSettingToRemote(localWebSiteId, node);
                    }
                }

                return false;
            }
        }

        public static void CheckInToRemote(SiteDb siteDb, string remoteServerUrl, string remoteServerUserName, string remoteServerPassword, Guid remoteWebSiteId)
        {
            var store = Stores.ClusterNodes(siteDb);

            var localSelfRecord = store.Where(o => o.IsLocal && o.ServerWebSiteId == siteDb.WebSite.Id).FirstOrDefault();
            if (localSelfRecord == null)
            {
                return;
            }

            ClusterNode node = new ClusterNode
            {
                ServerUrl = localSelfRecord.ServerUrl,
                ServerWebSiteId = siteDb.WebSite.Id,
                UserName = localSelfRecord.UserName,
                Password = localSelfRecord.Password
            };

            CheckInToRemote(siteDb, remoteServerUrl, remoteServerUserName, remoteServerPassword, remoteWebSiteId, node);
        }

        private static void CheckInToRemote(SiteDb siteDb, string remoteServerUrl, string remoteServerUserName, string remoteServerPassword, Guid remoteWebSiteId, ClusterNode node)
        {
            string data = Lib.Helper.JsonHelper.Serialize(node);

            string url = remoteServerUrl + ClusterService.ClusterSyncSettingUrl + "?siteid=" + remoteWebSiteId.ToString();

            bool submitok = true;

            try
            {
                submitok = Kooboo.Lib.Helper.HttpHelper.PostData(url, null, System.Text.Encoding.UTF8.GetBytes(data), remoteServerUserName, remoteServerPassword);
            }
            catch (Exception)
            {
                submitok = false;
            }

            if (!submitok)
            {
                Sites.TaskQueue.QueueManager.Add(new TaskQueue.HttpPostStringContent() { RemoteUrl = url, UserName = remoteServerUserName, Password = remoteServerPassword, StringContent = data }, siteDb.WebSite.Id);
            }
        }

        internal static string GetLocalServerUrl(Guid webSiteId)
        {
            var website = Data.GlobalDb.WebSites.Get(webSiteId);

            var binding = Data.GlobalDb.Bindings.GetByWebSite(webSiteId).FirstOrDefault();

            string starturl = string.Empty;
            if (binding != null)
            {
                starturl = "http://";
                if (!string.IsNullOrEmpty(binding.SubDomain))
                {
                    starturl = starturl + binding.SubDomain + ".";
                }
                var domain = Data.GlobalDb.Domains.Get(binding.DomainId);
                if (domain == null)
                {
                    return string.Empty;
                }
                starturl = starturl + domain.DomainName;
            }
            else
            {
                var ip = Lib.Helper.NetworkHelper.GetLocalIpAddress();
                if (string.IsNullOrEmpty(ip))
                {
                    return null;
                }

                starturl = "http://" + ip;
            }

            if (AppSettings.CurrentUsedPort != 80 && AppSettings.CurrentUsedPort > 0)
            {
                starturl = starturl + ":" + AppSettings.CurrentUsedPort;
            }

            return starturl;
        }

        public static void SyncSettingToRemote(Guid localWebSiteId, ClusterNode node)
        {
            var website = Data.GlobalDb.WebSites.Get(localWebSiteId);
            if (website == null || !website.EnableCluster)
            {
                return;
            }

            var synocto = ClusterService.GetClusterTo(website);

            var store = Stores.ClusterNodes(website.SiteDb());

            foreach (var item in synocto)
            {
                var server = store.get(item);
                if (server != null)
                {
                    CheckInToRemote(website.SiteDb(), server.ServerUrl, server.UserName, server.Password, server.ServerWebSiteId, node);
                }
            }
        }

        public static Sync.SyncObject GetSyncObject(SiteDb siteDb, LogEntry log, bool generateClusterInfo = true)
        {
            var key = Kooboo.IndexedDB.ByteConverter.GuidConverter.ConvertFromByte(log.KeyBytes);

            Sync.SyncObject item = new Sync.SyncObject();

            var repo = siteDb.GetRepository(log.StoreName);

            var siteobject = repo.GetByLog(log) as ISiteObject;

            if (log.EditType == EditType.Delete)
            {
                item.IsDelete = true;
                item.ObjectConstType = ConstTypeContainer.GetConstType(repo.ModelType);
                item.ObjectId = key;
            }
            else
            {
                if (siteobject != null)
                {
                    item = Sync.SyncObjectConvertor.ToSyncObject(siteobject);
                }
            }

            if (generateClusterInfo)
            {
                Sync.Cluster.Integrity.Generate(siteDb, item, log.Id);
            }

            item.SenderVersion = log.Id;

            return item;
        }

        public static void ReceiveRemote(SiteDb siteDb, Sync.SyncObject syncObject)
        {
            if (Integrity.Verify(siteDb, syncObject))
            {
                var modeltype = Service.ConstTypeService.GetModelType(syncObject.ObjectConstType);
                var repo = siteDb.GetRepository(modeltype);

                ISiteObject siteobject;

                if (syncObject.IsDelete)
                {
                    siteobject = Activator.CreateInstance(modeltype) as ISiteObject;
                    repo.Delete(syncObject.ObjectId);
                    if (siteobject != null)
                    {
                        siteobject.ConstType = syncObject.ObjectConstType;
                        siteobject.Id = syncObject.ObjectId;
                    }
                }
                else
                {
                    siteobject = Sync.SyncObjectConvertor.FromSyncObject(syncObject);
                    repo.AddOrUpdate(siteobject);
                }

                Integrity.AddHistory(siteDb, syncObject, siteobject);
            }
        }

        public static bool PostToRemote(SiteDb siteDb, LogEntry log, Guid remoteClusterId)
        {
            string url = ClusterService.ClusterUpdateUrl;

            url = url + "?siteid=" + siteDb.Id.ToString();

            var store = Stores.ClusterNodes(siteDb);

            var server = store.get(remoteClusterId);

            if (server == null)
            { return false; }

            url = server.ServerUrl + url;

            var item = GetSyncObject(siteDb, log);

            if (item == null)
            {
                return false;
            }

            string data = Lib.Helper.JsonHelper.Serialize(item);

            return Kooboo.Lib.Helper.HttpHelper.PostData(url, null, System.Text.Encoding.UTF8.GetBytes(data), server.UserName, server.Password);
        }
    }
}