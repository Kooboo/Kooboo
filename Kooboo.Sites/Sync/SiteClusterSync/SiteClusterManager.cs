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

        private bool _shouldNotifyDns = false;

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

            if (!(value is ICoreObject core))
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
                        var sameItems = this.PushQueue.Where(o => o.ClusterId == task.ClusterId && o.ObjectId == task.ObjectId).ToList();
                        if (sameItems == null || sameItems.Count == 0)
                        {
                            this.PushQueue.Add(task);
                        }
                        else
                        {
                            if (task.IsDelete)
                            {
                                foreach (var item in sameItems)
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
                if (this.PushQueue.Any())
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
            get { return _sitecluster ?? (_sitecluster = this.SiteDb.SiteCluster.All()); }
            set { _sitecluster = value; }
        }

        public void ResetCluster()
        {
            this._sitecluster = null;
        }

        public void SetClusterVersion(Guid clusterId, long version)
        {
            lock (_queuelocker)
            {
                var curent = this.SiteCluster.Find(o => o.Id == clusterId);
                if (curent != null)
                {
                    this.SiteDb.SiteCluster.UpdateVersion(clusterId, version);
                    curent.Version = version;
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
                            // ignored
                        }

                        IsRunning = false;

                        if (_shouldNotifyDns)
                        {
                            NotifyDns();
                        }

                        _shouldNotifyDns = false;
                    }
                }
            }
        }

        internal void NotifyDns()
        {
            if (Kooboo.Data.AppSettings.Global.IsOnlineServer)
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

                PostSyncObject postobject = new PostSyncObject
                {
                    SyncObject = syncobject, RemoteSiteId = this.SiteDb.WebSite.Id
                };


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
                    catch (Exception)
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
            return Kooboo.Data.AppSettings.CurrentUsedPort;
        }

        public SyncObject GetSyncObject(PushTask task)
        {
            SyncObject syncobject = null;

            if (task.IsDelete)
            {
                syncobject = new SyncObject {IsDelete = true, ObjectId = task.ObjectId};
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
            return repo?.StoreName;
        }

        public void Receive(SyncObject syncObject, string clientIp)
        {
            SiteCluster node = null;

            foreach (var item in this.SiteCluster)
            {
                if (Lib.Helper.IPHelper.IsInSameCClass(clientIp, item.ServerIp))
                {
                    node = item;
                    break;
                }
            }

            if (node != null)
            {
                Receive(syncObject, node);
            }
        }

        public void Receive(SyncObject syncObject, SiteCluster fromNode)
        {
            var repo = this.SiteDb.GetRepository(syncObject.StoreName);

            if (repo == null)
            {
                return;
            }

            this.Control.LockItem(fromNode.Id, syncObject.ObjectId);

            if (syncObject.IsDelete)
            {
                repo.Delete(syncObject.ObjectId);
            }
            else
            {
                var siteobject = SyncObjectConvertor.FromSyncObject(syncObject);
                if (siteobject is ICoreObject core)
                {
                    core.Version = -1;
                    repo.AddOrUpdate(core);
                }
            }

            this.Control.UnlockItem(fromNode.Id, syncObject.ObjectId);
        }

        private long GetJustDeltedVersion(IRepository repo, Guid objectId)
        {
            byte[] key = Service.ObjectService.KeyConverter.ToByte(objectId);
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
            _shouldNotifyDns = true;
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
                    PushTask task = new PushTask
                    {
                        ObjectId = this.KeyConverter.FromByte(log.KeyBytes),
                        IsDelete = log.EditType == EditType.Delete,
                        StoreName = log.StoreName,
                        Version = log.Id
                    };
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
                hashcode = Lib.Security.Hash.ComputeInt(unique);
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

        public void LockItem(Guid clusterId, Guid objectId)
        {
            var id = GetId(clusterId, objectId);
            lock (_locker)
            {
                Items.Add(id);
            }
        }

        public void UnlockItem(Guid clusterId, Guid objectId)
        {
            var id = GetId(clusterId, objectId);

            lock (_locker)
            {
                Items.Remove(id);
            }
        }

        public bool HasLock(Guid clusterId, Guid objectId)
        {
            var id = GetId(clusterId, objectId);

            lock (_locker)
            {
                return Items.Contains(id);
            }
        }
    }
}