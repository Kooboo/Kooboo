//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.TaskQueue.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync.SiteClusterSync
{
    public class SiteClusterManager
    {
        private SiteDb SiteDb { get; set; }

        private IByteConverter<Guid> KeyConverter { get; set; } = ObjectContainer.GetConverter<Guid>();
          
        public ControlFlow Control { get; set; } = new ControlFlow(); 
         
        private object _queuelocker = new object();

        private object _locktask = new object();
        public bool IsRunning { get; set; } = false;

        private bool ShouldNotifyDns = false;

        public SiteClusterManager(SiteDb sitedb)
        {
            this.SiteDb = sitedb;
        }

        public SortedSet<PushTask> PushQueue { get; set; } = new SortedSet<PushTask>();

        // Add from SiteRepositoryBase Event.... 
        public void AddTask(IRepository repo, ISiteObject value, ChangeType changetype)
        {
            if (repo == null || value == null)
            {
                return;
            }
            var core = value as ICoreObject;
            if (core == null)
            {
                return;
            }

            lock (_queuelocker)
            {
                PushTask task = new PushTask() { StoreName = repo.StoreName, ObjectId = value.Id };

                if (changetype == ChangeType.Delete)
                {
                    task.Version = GetJustDeltedVersion(repo, value.Id);
                    task.IsDelete = true;
                }
                else
                {
                    task.Version = core.Version;
                } 
                this.AddTask(task);
            }
        }

        public void AddTask(PushTask intask)
        {
            lock (_queuelocker)
            {
                foreach (var cluster in this.SiteCluster)
                { 
                    if (!this.Control.HasLock(cluster.Id, intask.ObjectId))
                    {
                        // clone a new task. 
                        PushTask task = new PushTask() { ObjectId = intask.ObjectId, Version = intask.Version, StoreName = intask.StoreName, ClusterId = cluster.Id, IsDelete = intask.IsDelete };

                        // check for each site cluster.. 
                        var SameItems = this.PushQueue.Where(o => o.ClusterId == task.ClusterId && o.ObjectId == task.ObjectId).ToList();
                        if (SameItems == null || SameItems.Count == 0)
                        {
                            this.PushQueue.Add(task);
                        }
                        else
                        {
                            if (task.IsDelete)
                            {
                                foreach (var item in SameItems)
                                {
                                    this.PushQueue.Remove(item);
                                }
                            }
                            this.PushQueue.Add(task);
                        }
                    }
                }
            }
            EnsureStart();
        }

        public PushTask PeekTask()
        {
            lock (_queuelocker)
            {
                if (this.PushQueue.Count() > 0)
                {
                    var item = this.PushQueue.First();
                    this.PushQueue.Remove(item);
                    return item;
                }
            }
            return default(PushTask);
        }

        private List<SiteCluster> _sitecluster;

        public List<SiteCluster> SiteCluster
        {
            get
            {
                if (_sitecluster == null)
                {
                    _sitecluster = this.SiteDb.SiteCluster.All(); 
                }
                return _sitecluster;
            }
            set { _sitecluster = value; }
        }

        public void ResetCluster()
        {
            this._sitecluster = null;
        }

        public void SetClusterVersion(Guid ClusterId, long Version)
        {
            lock (_queuelocker)
            {
                var curent = this.SiteCluster.Find(o => o.Id == ClusterId);
                if (curent != null)
                {
                    this.SiteDb.SiteCluster.UpdateVersion(ClusterId, Version);
                    curent.Version = Version;
                }
            }
        }

        private void ProcessTask()
        {
            if (!IsRunning)
            {
                lock (_locktask)
                {
                    if (!IsRunning)
                    {
                        IsRunning = true;

                        try
                        {
                            _Processtask();
                        }
                        catch (Exception)
                        {
                        }

                        IsRunning = false;

                        if (ShouldNotifyDns)
                        {
                            NotifyDns();
                        }

                        ShouldNotifyDns = false;
                    }
                }
            }

        }

        internal void NotifyDns()
        {
            if (Kooboo.Data.AppSettings.IsOnlineServer)
            {

            }
            // TODO: to be implemented.
        }

        private void _Processtask()
        {
            PushTask task = PeekTask();
            if (task.Version != default(long) && task.ClusterId != default(Guid))
            {
                SyncObject syncobject = GetSyncObject(task);

                PostSyncObject postobject = new PostSyncObject() { SyncObject = syncobject };

                postobject.RemoteSiteId = this.SiteDb.WebSite.Id;

                TaskQueue.TaskExecutor.PostSyncObjectTask executor = new TaskQueue.TaskExecutor.PostSyncObjectTask();

                foreach (var item in this.SiteCluster)
                {
                    postobject.RemoteUrl = ClusterUrl.Push(item);

                    try
                    {
                        if (!executor.Execute(this.SiteDb, Lib.Helper.JsonHelper.Serialize(postobject)))
                        {
                            TaskQueue.QueueManager.Add(postobject, this.SiteDb.WebSite.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskQueue.QueueManager.Add(postobject, this.SiteDb.WebSite.Id);
                        // TODO: log to system log. 
                    }
                }
                _Processtask();
            }
        }

        private int GetPort()
        {
            var bindings = Kooboo.Data.GlobalDb.Bindings.GetByWebSite(this.SiteDb.WebSite.Id);
            foreach (var item in bindings)
            {
                if (item.Port != 80 && item.Port > 0)
                {
                    return item.Port;
                }
            }
            return Kooboo.Data.AppSettings.HttpPort;
        }

        public SyncObject GetSyncObject(PushTask task)
        {
            SyncObject syncobject = null;

            if (task.IsDelete)
            {
                syncobject = new SyncObject();
                syncobject.IsDelete = true;
                syncobject.ObjectId = task.ObjectId;
            }
            else
            {
                var repo = this.SiteDb.GetRepository(task.StoreName);
                if (repo == null)
                {
                    return null;
                }

                var log = this.SiteDb.Log.Get(task.Version);
                if (log == null)
                {
                    return null;
                }
                var siteobject = repo.GetByLog(log);
                syncobject = SyncObjectConvertor.ToSyncObject(siteobject);
            }

            syncobject.StoreName = task.StoreName;
            syncobject.SenderPort = GetPort();
            return syncobject;
        }

        private string GetStoreName(byte consttype)
        {
            var repo = this.SiteDb.GetRepository(consttype);
            return repo != null ? repo.StoreName : null;
        }

        public void Receive(SyncObject SyncObject, string ClientIp)
        {
            SiteCluster node = null;

            foreach (var item in this.SiteCluster)
            {
                if (Lib.Helper.IPHelper.IsInSameCClass(ClientIp, item.ServerIp))
                {
                    node = item;
                    break;
                }
            }

            if (node != null)
            {
                Receive(SyncObject, node);
            }
        }

        public void Receive(SyncObject SyncObject, SiteCluster FromNode)
        {
            var repo = this.SiteDb.GetRepository(SyncObject.StoreName);

            if (repo == null)
            {
                return;
            }
             
            this.Control.LockItem(FromNode.Id, SyncObject.ObjectId); 
             
            if (SyncObject.IsDelete)
            {
                repo.Delete(SyncObject.ObjectId);
 
            }
            else
            {
                var siteobject = SyncObjectConvertor.FromSyncObject(SyncObject);
                if (siteobject is ICoreObject)
                {
                    var core = siteobject as ICoreObject;
                    core.Version = -1;
                    repo.AddOrUpdate(core); 
                }
            }

            this.Control.UnlockItem(FromNode.Id, SyncObject.ObjectId); 
        }

        private long GetJustDeltedVersion(IRepository repo, Guid ObjectId)
        {
            byte[] key = Service.ObjectService.KeyConverter.ToByte(ObjectId);
            var oldlogs = SiteDb.Log.GetByStoreNameAndKey(repo.StoreName, key, 1);
            if (oldlogs == null || oldlogs.Count() == 0)
            { return -1; }
            var log = oldlogs.First();
            if (log.EditType == IndexedDB.EditType.Delete)
            {
                return log.Id;
            }

            var logs = SiteDb.Log.GetByStoreNameAndKey(repo.StoreName, key, 10);
            foreach (var item in logs.OrderByDescending(o => o.Id))
            {
                if (item.EditType == IndexedDB.EditType.Delete)
                {
                    return item.Id;
                }
            }
            return -1;
        }

        public void InitStart()
        {
            ShouldNotifyDns = true;
            InitTask();
            EnsureStart();
        }

        public void EnsureStart()
        {
            if (!IsRunning)
            {
                Task.Factory.StartNew(ProcessTask);
            }
        }

        internal void InitTask()
        {
            foreach (var item in this.SiteCluster)
            {
                var alllogs = GetLogItems(item);

                foreach (var log in alllogs.OrderBy(o => o.Id))
                {
                    PushTask task = new PushTask();
                    task.ObjectId = this.KeyConverter.FromByte(log.KeyBytes);
                    task.IsDelete = log.EditType == EditType.Delete;
                    task.StoreName = log.StoreName;
                    task.Version = log.Id;
                    this.AddTask(task);
                }
            }
        }

        //Get items to push to remote.. based on the log.... this can only be push from lower to higher... 
        public List<LogEntry> GetLogItems(Guid clusterId)
        {
            var cluster = this.SiteCluster.Find(o => o.Id == clusterId);
            return GetLogItems(cluster);
        }

        public List<LogEntry> GetLogItems(SiteCluster cluster)
        {
            if (cluster == null)
            {
                return new List<LogEntry>();
            }

            var keyconverter = ObjectContainer.GetConverter<Guid>();

            var currentlogid = cluster.Version;

            List<LogEntry> result = new List<LogEntry>();

            var alllogs = this.SiteDb.Log.Store.Where(o => o.Id > currentlogid).SelectAll();

            foreach (var item in alllogs.OrderByDescending(o => o.Id))
            {
                if (item.KeyBytes == null)
                {
                    continue;
                }
                Guid key = keyconverter.FromByte(item.KeyBytes);
                var currentrecord = result.Find(o => o.KeyHash == item.KeyHash);
                if (currentrecord == null)
                {
                    result.Add(item);
                }
                else
                {
                    if (item.EditType == EditType.Add)
                    {
                        if (currentrecord.EditType == EditType.Delete)
                        {
                            // new added item also get deleted at the end...==> should not push to it.
                            result.Remove(currentrecord);
                        }
                        else if (currentrecord.EditType == EditType.Update)
                        {
                            currentrecord.EditType = EditType.Add;
                        }
                    }
                }

            }
            return result;
        }
    }

    public struct PushTask : IComparable<PushTask>
    {
        public bool IsDelete { get; set; }

        public Guid ObjectId { get; set; }

        public string StoreName { get; set; }

        public Guid ClusterId { get; set; }

        public long Version { get; set; }

        public int CompareTo(PushTask other)
        {
            var diff = this.Version - other.Version;

            var result = (int)diff;

            if (result == 0)
            {
                return this.GetHashCode() - other.GetHashCode();
            }
            return result;
        }

        internal int hashcode;

        public override int GetHashCode()
        {
            if (hashcode == default(int))
            {
                string unique = this.IsDelete.ToString() + this.ObjectId.ToString() + this.StoreName + this.ClusterId.ToString() + this.Version.ToString();
                hashcode = Lib.Security.Hash.ComputeIntCaseSensitive(unique);
            }
            return hashcode;
        }
    }
      
    public class ControlFlow
    {
        private HashSet<Guid> Items = new HashSet<Guid>();

        public object _locker = new object();

        private Guid GetId(Guid one, Guid two)
        {   
            byte[] data = new byte[32];
            one.ToByteArray().CopyTo(data, 0);
            two.ToByteArray().CopyTo(data, 16);

            byte[] hash = MD5.Create().ComputeHash(data);
            return new Guid(hash);
        }
        public void LockItem(Guid ClusterId, Guid ObjectId)
        {
            var id = GetId(ClusterId, ObjectId);
            lock(_locker)
            {
                Items.Add(id); 
            } 
        }

        public void UnlockItem(Guid ClusterId, Guid ObjectId)
        {
            var id = GetId(ClusterId, ObjectId);

            lock(_locker)
            {
                Items.Remove(id); 
            } 
        }

        public bool HasLock(Guid ClusterId, Guid ObjectId)
        {
            var id = GetId(ClusterId, ObjectId); 

            lock(_locker)
            {
                return Items.Contains(id); 
            }
        }

    }

}