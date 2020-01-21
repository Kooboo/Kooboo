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

        public static List<Guid> GetClusterTo(Data.Models.WebSite WebSite)
        {
            lock (_locker)
            {
                if (!ClusterTo.ContainsKey(WebSite.Id))
                {
                    var tos = CalculateClusterTo(WebSite);
                    ClusterTo[WebSite.Id] = tos;
                }
                return ClusterTo[WebSite.Id];
            }
        }

        internal static List<Guid> CalculateClusterTo(Data.Models.WebSite WebSite)
        {
            HashSet<Guid> result = new HashSet<Guid>();

            var store = Stores.ClusterNodes(WebSite.SiteDb());

            var allitems = store.Filter.SelectAll().OrderBy(o => o.ServerUrl).ToList();

            var count = allitems.Count();
            if (count == 0)
            {
                return new List<Guid>();
            }
            int currentposition = -1;

            for (int i = 0; i < count; i++)
            {
                if (allitems[i].ServerWebSiteId == WebSite.Id)
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


        public static void EnableLocalSite(Guid LocalWebSiteId, string LocalUrl, string LocalUserName, string LocalPassword)
        {
            lock (_locker)
            {
                var website = Data.GlobalDb.WebSites.Get(LocalWebSiteId);
                if (website != null)
                {
                    if (!website.EnableCluster)
                    {
                        website.EnableCluster = true;
                        Data.GlobalDb.WebSites.AddOrUpdate(website);
                        website = Data.GlobalDb.WebSites.Get(LocalWebSiteId);
                    }

                    var store = Stores.ClusterNodes(website.SiteDb());

                    if (string.IsNullOrEmpty(LocalUrl))
                    {
                        LocalUrl = GetLocalServerUrl(LocalWebSiteId);
                    }

                    AddOrUpdate(store, LocalUrl, LocalUserName, LocalPassword, LocalWebSiteId, true);

                    ClusterTo.Remove(LocalWebSiteId);
                }
            }
        }


        /// <summary>
        /// Add remote server and then need to PostToCheckIn. 
        /// </summary>
        /// <param name="LocalWebSiteId"></param>
        /// <param name="RemoteServerUrl"></param>
        /// <param name="RemoteServerUserName"></param>
        /// <param name="RemoteServerPassword"></param>
        /// <param name="RemoteWebSiteId"></param>
        public static void AddRemoteServer(Guid LocalWebSiteId, string RemoteServerUrl, string RemoteServerUserName, string RemoteServerPassword, Guid RemoteWebSiteId)
        {
            lock (_locker)
            {
                var website = Data.GlobalDb.WebSites.Get(LocalWebSiteId);
                if (website != null)
                {
                    if (!website.EnableCluster)
                    {
                        website.EnableCluster = true;
                        Data.GlobalDb.WebSites.AddOrUpdate(website);
                        website = Data.GlobalDb.WebSites.Get(LocalWebSiteId);
                    }

                    var store = Stores.ClusterNodes(website.SiteDb());

                    AddOrUpdate(store, RemoteServerUrl, RemoteServerUserName, RemoteServerPassword, RemoteWebSiteId);

                    ClusterTo.Remove(LocalWebSiteId);
                }
            }
        }

        internal static ClusterNode AddOrUpdate(ObjectStore<Guid, ClusterNode> store, string ServerUrl, string UserName, string Password, Guid TargetWebSiteId, bool IsLocal = false)
        {
           
            ClusterNode item = new ClusterNode();
            item.ServerUrl = ServerUrl;
            item.UserName = UserName;
            item.Password = Password;
            item.ServerWebSiteId = TargetWebSiteId;
            item.IsLocal = IsLocal;

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
         
        public static bool CheckInByRemote(Guid LocalWebSiteId, ClusterNode RemoteNode)
        {
            lock (_locker)
            {
                var website = Data.GlobalDb.WebSites.Get(LocalWebSiteId);
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

                    var node = AddOrUpdate(store, RemoteNode.ServerUrl, RemoteNode.UserName, RemoteNode.Password, RemoteNode.ServerWebSiteId);
                    if (node != null)
                    {
                        SyncSettingToRemote(LocalWebSiteId, node);
                    }
                }

                return false;
            }
        }

        public static void CheckInToRemote(SiteDb SiteDb, string RemoteServerUrl, string RemoteServerUserName, string RemoteServerPassword, Guid RemoteWebSiteId)
        {
            var store = Stores.ClusterNodes(SiteDb);

            var LocalSelfRecord = store.Where(o => o.IsLocal && o.ServerWebSiteId == SiteDb.WebSite.Id).FirstOrDefault();
            if (LocalSelfRecord == null)
            {
                return;
            }

            ClusterNode node = new ClusterNode();
            node.ServerUrl = LocalSelfRecord.ServerUrl;
            node.ServerWebSiteId = SiteDb.WebSite.Id;
            node.UserName = LocalSelfRecord.UserName;
            node.Password = LocalSelfRecord.Password;

            CheckInToRemote(SiteDb, RemoteServerUrl, RemoteServerUserName, RemoteServerPassword, RemoteWebSiteId, node);
        }

        private static void CheckInToRemote(SiteDb SiteDb, string RemoteServerUrl, string RemoteServerUserName, string RemoteServerPassword, Guid RemoteWebSiteId, ClusterNode node)
        {
            string data = Lib.Helper.JsonHelper.Serialize(node);

            string url = RemoteServerUrl + ClusterService.ClusterSyncSettingUrl + "?siteid=" + RemoteWebSiteId.ToString();

            bool submitok = true;

            try
            {
                submitok =  Kooboo.Lib.Helper.HttpHelper.PostData(url, null, System.Text.Encoding.UTF8.GetBytes(data), RemoteServerUserName, RemoteServerPassword);  
            }
            catch (Exception)
            {
                submitok = false;
            }

            if (!submitok)
            {
                Sites.TaskQueue.QueueManager.Add(new TaskQueue.HttpPostStringContent() { RemoteUrl = url, UserName = RemoteServerUserName, Password = RemoteServerPassword, StringContent = data }, SiteDb.WebSite.Id);  
            }
        }

        internal static string GetLocalServerUrl(Guid WebSiteId)
        {
            var website = Data.GlobalDb.WebSites.Get(WebSiteId);

            var binding = Data.GlobalDb.Bindings.GetByWebSite(WebSiteId).FirstOrDefault();

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

            if (AppSettings.HttpPort != 80 && AppSettings.HttpPort > 0)
            {
                starturl = starturl + ":" + AppSettings.HttpPort;
            }

            return starturl; 
        }

        public static void  SyncSettingToRemote(Guid LocalWebSiteId, ClusterNode node)
        {
            var website = Data.GlobalDb.WebSites.Get(LocalWebSiteId);
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

        public static Sync.SyncObject GetSyncObject(SiteDb SiteDb, LogEntry log, bool GenerateClusterInfo = true)
        {
            var key = Kooboo.IndexedDB.ByteConverter.GuidConverter.ConvertFromByte(log.KeyBytes);

            Sync.SyncObject item = new Sync.SyncObject();

            var repo = SiteDb.GetRepository(log.StoreName);

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

            if (GenerateClusterInfo)
            {
                Sync.Cluster.Integrity.Generate(SiteDb, item, log.Id);
            }

            item.SenderVersion = log.Id; 

            return item;
        }

        public static void ReceiveRemote(SiteDb SiteDb, Sync.SyncObject SyncObject)
        {
            if (Integrity.Verify(SiteDb, SyncObject))
            {
                var modeltype = Service.ConstTypeService.GetModelType(SyncObject.ObjectConstType);
                var repo = SiteDb.GetRepository(modeltype);

                ISiteObject siteobject;

                if (SyncObject.IsDelete)
                {
                    siteobject = Activator.CreateInstance(modeltype) as ISiteObject;
                    repo.Delete(SyncObject.ObjectId);
                    siteobject.ConstType = SyncObject.ObjectConstType;
                    siteobject.Id = SyncObject.ObjectId;
                }
                else
                {
                    siteobject = Sync.SyncObjectConvertor.FromSyncObject(SyncObject);
                    repo.AddOrUpdate(siteobject);
                }

                Integrity.AddHistory(SiteDb, SyncObject, siteobject);
            }

        }

        public static bool PostToRemote(SiteDb SiteDb, LogEntry log, Guid RemoteClusterId)
        {
            string url = ClusterService.ClusterUpdateUrl;

            url = url + "?siteid=" + SiteDb.Id.ToString();

            var store = Stores.ClusterNodes(SiteDb);

            var server = store.get(RemoteClusterId);

            if (server == null)
            { return false; }

            url = server.ServerUrl + url;

            var item = GetSyncObject(SiteDb, log);

            if (item == null)
            {
                return false;
            }

            string data = Lib.Helper.JsonHelper.Serialize(item);
             
            return Kooboo.Lib.Helper.HttpHelper.PostData(url, null, System.Text.Encoding.UTF8.GetBytes(data), server.UserName, server.Password); 
        } 
    }
}
