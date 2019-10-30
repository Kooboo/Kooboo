//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync
{
    public class DiskSyncManager
    {
        public SyncMediator SyncMediator { get; set; }

        private object _queuelocker { get; set; }

        public DiskSyncManager(Guid webSiteId)
        {
            this.WebSiteId = webSiteId;
            this.SyncMediator = new SyncMediator();
            _queuelocker = new object();
        }

        private int _maxThread = 10;
        private int _currentThreadCount = 0;

        private Guid WebSiteId { get; set; }

        private bool CanAccept
        {
            get
            { return this._currentThreadCount < _maxThread; }
        }

        public List<DiskChangeEvent> ChangeTasks { get; set; } = new List<DiskChangeEvent>();

        public bool AddTask(DiskChangeEvent changeItem)
        {
            lock (_queuelocker)
            {
                if (changeItem.ChangeType == DiskChangeType.Created && this.SyncMediator.IsDiskLock(changeItem.FullPath, int.MaxValue))
                {
                    return false;
                }
                var sameItems = this.ChangeTasks.FindAll(o => o.FullPath == changeItem.FullPath).ToList();

                if (sameItems == null || sameItems.Count == 0)
                {
                    this.ChangeTasks.Add(changeItem);
                    return true;
                }
                else
                {
                    if (changeItem.ChangeType == DiskChangeType.Deleted)
                    {
                        foreach (var item in sameItems)
                        {
                            if (!item.Peeked && item.ChangeType != DiskChangeType.Deleted)
                            {
                                this.ChangeTasks.Remove(item);
                            }
                        }

                        if (sameItems.Find(o => o.ChangeType == DiskChangeType.Deleted) == null)
                        {
                            this.ChangeTasks.Add(changeItem);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (changeItem.ChangeType == DiskChangeType.Rename)
                    {
                        var item = sameItems.Find(o => o.ChangeType == DiskChangeType.Rename && o.OldFullPath == changeItem.OldFullPath && !o.Peeked);

                        if (item != null)
                        {
                            item.FullPath = changeItem.FullPath;
                            return false;
                        }
                        else
                        {
                            this.ChangeTasks.Add(changeItem);
                            return true;
                        }
                    }
                    else
                    {
                        // if there is an update item to be executed, do nothing.
                        if (sameItems.Find(o => (o.ChangeType == DiskChangeType.Created || o.ChangeType == DiskChangeType.Updated)) != null)
                        {
                            return false;
                        }
                        else
                        {
                            this.ChangeTasks.Add(changeItem);
                            return true;
                        }
                    }
                }
            }
        }

        public DiskChangeEvent PeekTask()
        {
            lock (_queuelocker)
            {
                foreach (var item in this.ChangeTasks)
                {
                    if (!item.Peeked)
                    {
                        item.Peeked = true;
                        return item;
                    }
                }
            }
            return null;
        }

        public void RemoveTask(DiskChangeEvent change)
        {
            lock (_queuelocker)
            {
                this.ChangeTasks.RemoveAll(o => o.Id == change.Id);
            }
        }

        public void startNewFromDisk()
        {
            if (this.CanAccept && this.ChangeTasks.Count > 0)
            {
                Task.Factory.StartNew(ProcessFromDisk);
            }
        }

        internal void ProcessFromDisk()
        {
            if (this.CanAccept)
            {
                Interlocked.Increment(ref this._currentThreadCount);
                DiskChangeEvent task = PeekTask();

                if (task != null)
                {
                    try
                    {
                        Data.Models.WebSite website = Data.GlobalDb.WebSites.Get(WebSiteId);
                        var sitedb = website.SiteDb();

                        if (task.ChangeType == DiskChangeType.Rename)
                        {
                            if (DiskPathService.IsDirectory(website, task.OldFullPath) && DiskPathService.IsDirectory(website, task.FullPath))
                            {
                                SyncService.DiskFolderRename(sitedb, task.OldFullPath, task.FullPath);
                            }
                            else
                            {
                                SyncService.DiskFileRename(sitedb, task.OldFullPath, task.FullPath);
                            }
                        }
                        else if (task.ChangeType == DiskChangeType.Deleted)
                        {
                            if (DiskPathService.IsDirectory(website, task.FullPath))
                            {
                                SyncService.DeleteDiskFolder(task.FullPath, sitedb);
                            }
                            else
                            {
                                this.DeleteFromDb(task.FullPath, sitedb);
                            }
                        }
                        else
                        {
                            //if (task.ChangeType == DiskChangeType.Created)
                            //{
                            //    contentbytes = new byte[0];
                            //}

                            this.SyncToDb(task.FullPath, sitedb, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;
                    }

                    this.RemoveTask(task);
                }
                Interlocked.Decrement(ref this._currentThreadCount);

                if (task != null)
                {
                    startNewFromDisk();
                }
            }
        }

        public void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.OldFullPath) && !string.IsNullOrEmpty(e.FullPath))
            {
                DiskChangeEvent theevent = new DiskChangeEvent
                {
                    OldFullPath = e.OldFullPath, FullPath = e.FullPath, ChangeType = DiskChangeType.Rename
                };

                if (this.AddTask(theevent))
                {
                    startNewFromDisk();
                }
            }
        }

        private bool IsPathLock(string fullpath)
        {
            if (IsDirectory(fullpath))
            {
                return true;
            }

            if (this.SyncMediator.IsDiskLock(fullpath))
            {
                return true;
            }
            return false;
        }

        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (this.IsPathLock(e.FullPath))
            {
                return;
            }

            DiskChangeEvent theevent = new DiskChangeEvent {FullPath = e.FullPath};

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    theevent.ChangeType = DiskChangeType.Updated;
                    break;
                case WatcherChangeTypes.Created:
                    theevent.ChangeType = DiskChangeType.Created;
                    break;
                case WatcherChangeTypes.Deleted:
                    theevent.ChangeType = DiskChangeType.Deleted;
                    break;
                case WatcherChangeTypes.Renamed:
                    throw new Exception("should fire rename event");
            }

            if (this.AddTask(theevent))
            {
                startNewFromDisk();
            }
        }

        public void DeleteFromDb(string diskFullPath, SiteDb sitedb)
        {
            var nonRoutable = DiskPathService.GetNonRoutableObject(diskFullPath);

            if (nonRoutable != null)
            {
                var repo = sitedb.GetRepository(nonRoutable.StoreName);
                string name = string.IsNullOrWhiteSpace(nonRoutable.Name) ? nonRoutable.Id.ToString() : nonRoutable.Name;
                if (repo.GetByNameOrId(name) is ISiteObject result)
                {
                    this.SyncMediator.AcquireDbLock(result.Id);
                    repo.Delete(result.Id);
                    this.SyncMediator.ReleaseDbLock(result.Id);
                }
            }
            else
            {
                var relativeUrl = DiskPathService.GetRelativeUrl(sitedb.WebSite, diskFullPath);
                var route = sitedb.Routes.GetByUrl(relativeUrl);
                if (route != null)
                {
                    var repo = sitedb.GetRepository(route.DestinationConstType);
                    if (repo?.Get(route.objectId) is ISiteObject result)
                    {
                        this.SyncMediator.AcquireDbLock(result.Id);
                        repo.Delete(result.Id);
                        this.SyncMediator.ReleaseDbLock(result.Id);
                    }
                }
            }
        }

        public bool IsDirectory(string path)
        {
            var parts = path.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts == null || !parts.Any())
            {
                return false;
            }

            return parts[0].Contains(".");
        }

        public void SyncToDb(string fullPath, SiteDb SiteDb, byte[] diskbytes = null, bool logSync = true)
        {
            if (diskbytes == null)
            {
                diskbytes = this.ReadAllBytes(fullPath);
            }
            if (diskbytes == null)
            {
                return;
            }

            if (this.SyncMediator.IsContentHashLock(fullPath, diskbytes))
            {
                return;
            }

            //if (!this.SyncMediator.CheckAndAcquireDiskLock(FullPath, diskbytes))
            //{
            //    return;
            //}
            string oldRelativeUrl = null;
            string relativeUrl = null;

            IRepository repo = null;
            ISiteObject result = null;
            Routing.Route route = null;
            string nameFromFile = null;
            string extension = UrlHelper.FileExtension(fullPath);
            if (!string.IsNullOrEmpty(extension) && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.ToLower();
            }

            var nonRoutable = DiskPathService.GetNonRoutableObject(fullPath);

            if (nonRoutable != null)
            {
                repo = SiteDb.GetRepository(nonRoutable.StoreName);
                nameFromFile = nonRoutable.Name;
                string name = string.IsNullOrWhiteSpace(nonRoutable.Name) ? nonRoutable.Id.ToString() : nonRoutable.Name;
                if (!string.IsNullOrEmpty(nonRoutable.Extension))
                {
                    extension = nonRoutable.Extension.ToLower();
                    if (!extension.StartsWith("."))
                    {
                        extension = "." + extension;
                    }
                }

                result = repo.GetByNameOrId(name) as ISiteObject;

                if (result == null)
                {
                    if (name.ToLower().EndsWith(extension))
                    {
                        name = name.Substring(0, name.Length - extension.Length);
                        result = repo.GetByNameOrId(name);
                    }
                    else
                    {
                        name = name + extension;
                        result = repo.GetByNameOrId(name);
                    }
                }
            }
            else
            {
                oldRelativeUrl = DiskPathService.GetRelativeUrl(SiteDb.WebSite, fullPath);
                relativeUrl = Kooboo.Sites.Helper.RouteHelper.ToValidRoute(oldRelativeUrl);

                route = SiteDb.Routes.GetByUrl(relativeUrl);
                if (route != null)
                {
                    repo = SiteDb.GetRepository(route.DestinationConstType);
                    result = repo.Get(route.objectId) as ISiteObject;
                }
                else
                {
                    var modelType = Service.ConstTypeService.GetModelTypeByUrl(relativeUrl);
                    if (modelType == null) { return; }
                    repo = SiteDb.GetRepository(modelType);
                }
                nameFromFile = UrlHelper.FileName(relativeUrl);
            }

            if (result == null)
            {
                result = Activator.CreateInstance(repo.ModelType) as ISiteObject;
            }

            if (!CheckAssignObject(ref result, diskbytes))
            {
                return;
            }

            if (result is IExtensionable extensionfile)
            {
                extensionfile.Extension = extension;
            }

            if (string.IsNullOrEmpty(result.Name))
            {
                result.Name = Lib.Helper.StringHelper.ToValidFileName(nameFromFile);
            }

            if (!string.IsNullOrEmpty(relativeUrl))
            {
                SiteDb.Routes.AddOrUpdate(relativeUrl, result as SiteObject);
            }

            if (!IsSameName(result.Name, nameFromFile, extension) || oldRelativeUrl != relativeUrl)
            {
                if (File.Exists(fullPath))
                {
                    this.SyncMediator.AbsoluteLock(fullPath);

                    File.Delete(fullPath);

                    this.SyncMediator.LockDisk3Seconds(fullPath);
                    this.SyncMediator.ReleaseAbsoluteLock(fullPath);
                }

                repo.AddOrUpdate(result);
            }
            else

            {
                this.SyncMediator.AcquireDbLock(result.Id);

                repo.AddOrUpdate(result);

                this.SyncMediator.ReleaseDbLock(result.Id);
            }

            if (logSync)
            {
                var coreobject = result as CoreObject;
                SiteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = SiteDb.Synchronization.DiskSyncSettingId, ObjectId = coreobject.Id, Version = coreobject.Version, StoreName = repo.StoreName });
            }
        }

        private bool IsSameName(string x, string y, string extension)
        {
            extension = extension.ToLower();
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            if (x == null || y == null)
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(extension))
            {
                return x.ToLower() == y.ToLower();
            }
            else
            {
                if (x.ToLower().EndsWith(extension))
                {
                    x = x.Substring(0, x.Length - extension.Length);
                }

                if (y.ToLower().EndsWith(extension))
                {
                    y = y.Substring(0, y.Length - extension.Length);
                }

                return x.ToLower() == y.ToLower();
            }
        }

        public bool CheckAssignObject(ref ISiteObject siteObject, byte[] diskBytes)
        {
            var modeltype = siteObject.GetType();
            var serializerType = Attributes.AttributeHelper.GetDiskType(modeltype);

            if (serializerType == Kooboo.Attributes.DiskType.Binary)
            {
                var binaryfile = siteObject as IBinaryFile;
                if (diskBytes == null || IOHelper.IsEqualBytes(binaryfile?.ContentBytes, diskBytes))
                {
                    return false;
                }
                binaryfile.ContentBytes = diskBytes;
            }
            else if (serializerType == Kooboo.Attributes.DiskType.Text)
            {
                var textfile = siteObject as ITextObject;
                string textbody = System.Text.Encoding.UTF8.GetString(diskBytes);
                if (StringHelper.IsSameValue(textbody, textfile?.Body))
                {
                    return false;
                }
                textfile.Body = textbody;
            }
            else
            {
                string fulltext = System.Text.Encoding.UTF8.GetString(diskBytes);
                var generatedbody = Lib.Helper.JsonHelper.Serialize(siteObject);

                if (StringHelper.IsSameValue(generatedbody, fulltext))
                {
                    return false;
                }
                siteObject = JsonHelper.Deserialize(fulltext, modeltype) as ISiteObject;
            }
            return true;
        }

        public string SyncToDisk(SiteDb siteDb, ISiteObject Value, ChangeType changeType, string storeName)
        {
            string diskpath = null;

            if (Attributes.AttributeHelper.IsDiskable(Value) && !IsEmbedded(Value) && !string.IsNullOrEmpty(storeName))
            {
                if (!this.SyncMediator.CheckDbLocked(Value.Id))
                {
                    var value = Value as ISiteObject;
                    string relativeurl = DiskPathService.GetObjectRelativeUrl(value, siteDb, storeName);

                    if (!string.IsNullOrEmpty(relativeurl))
                    {
                        string fullpath = DiskPathService.GetFullDiskPath(siteDb.WebSite, relativeurl);

                        if (changeType == ChangeType.Delete)
                        {
                            if (File.Exists(fullpath))
                            {
                                diskpath = fullpath;

                                // this.SyncMediator.AcquireDeletionLock(fullpath);

                                this.SyncMediator.AbsoluteLock(fullpath);

                                File.Delete(fullpath);

                                this.SyncMediator.LockDisk3Seconds(fullpath);
                                this.SyncMediator.ReleaseAbsoluteLock(fullpath);
                            }
                        }
                        else
                        {
                            if (value is ICoreObject coreobject)
                            {
                                bool hasPast = false;

                                var logs = siteDb.Synchronization.Query.Where(o => o.SyncSettingId == siteDb.Synchronization.DiskSyncSettingId && o.ObjectId == value.Id).SelectAll();

                                foreach (var item in logs)
                                {
                                    if (StringHelper.IsSameValue(storeName, item.StoreName))
                                    {
                                        if (item.Version >= coreobject.Version)
                                        {
                                            hasPast = true;
                                        }
                                    }
                                }

                                if (!hasPast)
                                {
                                    var contentbytes = SyncService.GetObjectBytes(value);

                                    if (File.Exists(fullpath))
                                    {
                                        var bytes = IOHelper.ReadAllBytes(fullpath);
                                        diskpath = fullpath;

                                        if (!IOHelper.IsEqualBytes(bytes, contentbytes))
                                        {
                                            this.SyncMediator.AbsoluteLock(fullpath);

                                            this.SyncMediator.ContentHashLock(fullpath, contentbytes);

                                            this.WriteBytes(fullpath, contentbytes);

                                            this.SyncMediator.LockDisk3Seconds(fullpath);
                                            this.SyncMediator.ReleaseAbsoluteLock(fullpath);
                                        }
                                    }
                                    else
                                    {
                                        // this.SyncMediator.AcquireDiskWriteLock(fullpath, contentbytes);

                                        // this.SyncMediator.LockDisk3Seconds(fullpath);

                                        this.SyncMediator.AbsoluteLock(fullpath);
                                        this.SyncMediator.ContentHashLock(fullpath, contentbytes);

                                        this.WriteBytes(fullpath, contentbytes);

                                        this.SyncMediator.LockDisk3Seconds(fullpath);
                                        this.SyncMediator.ReleaseAbsoluteLock(fullpath);

                                        diskpath = fullpath;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Value is ICoreObject core)
            {
                siteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = siteDb.Synchronization.DiskSyncSettingId, ObjectId = Value.Id, Version = core.Version, StoreName = storeName });
            }

            return diskpath;
        }

        public void InitSyncToDisk()
        {
            var website = Kooboo.Data.GlobalDb.WebSites.Get(this.WebSiteId);
            var sitedb = website.SiteDb();

            var allrepos = sitedb.ActiveRepositories();
            foreach (var repo in allrepos)
            {
                if (Kooboo.Attributes.AttributeHelper.IsDiskable(repo.ModelType))
                {
                    var allitems = repo.All();

                    foreach (var item in allitems)
                    {
                        SyncToDisk(sitedb, item, ChangeType.Add, repo.StoreName);
                    }
                }
            }
        }

        private bool IsEmbedded(ISiteObject value)
        {
            if (value is IEmbeddable embedded)
            {
                return embedded.IsEmbedded;
            }
            return false;
        }

        private object _IOLocker = new object();

        private byte[] ReadAllBytes(string filePath)
        {
            lock (_IOLocker)
            {
                if (File.Exists(filePath))
                {
                    int i = 0;
                    System.Threading.Thread.Sleep(10);  //TODO:  this is very strange action, otherwise, file will be being used.
                    while (i < 10)
                    {
                        try
                        {
                            if (File.Exists(filePath))
                            {
                                var bytes = File.ReadAllBytes(filePath);
                                return bytes;
                            }
                        }
                        catch (Exception)
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                        i += 1;
                    }
                }
                return null;
            }
        }

        internal void WriteBytes(string fullPath, byte[] value)
        {
            if (value == null)
            {
                value = new byte[0];
            }
            lock (_IOLocker)
            {
                IOHelper.EnsureFileDirectoryExists(fullPath);

                int i = 0;
                while (i < 10)
                {
                    try
                    {
                        System.IO.FileStream stream = new FileStream(fullPath, FileMode.Create);

                        stream.Write(value, 0, value.Length);

                        stream.Close();
                        stream.Dispose();
                    }
                    catch (Exception)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    i += 1;
                }
            }
        }
    }

    public class DiskChangeEvent : IEquatable<DiskChangeEvent>
    {
        public Guid Id { get; set; } = System.Guid.NewGuid();

        // for rename.
        public string OldFullPath { get; set; }

        public string FullPath { get; set; }

        public DiskChangeType ChangeType { get; set; }

        public bool Peeked { get; set; }

        public Guid ObjectId { get; set; }

        public override int GetHashCode()
        {
            return Lib.Security.Hash.ComputeInt(this.Id.ToString());
        }

        public bool Equals(DiskChangeEvent other)
        {
            return this.Id == other.Id;
        }
    }

    public enum DiskChangeType
    {
        Created = 1,
        Updated = 2,
        Deleted = 3,
        Rename = 4
    }

    public class SyncMediator
    {
        private HashSet<Guid> dblock = new HashSet<Guid>();

        #region "prevent duplicate DB write back to disk"

        public void AcquireDbLock(Guid siteObjectId)
        {
            dblock.Add(siteObjectId);
        }

        public void ReleaseDbLock(Guid siteObjectId)
        {
            dblock.Remove(siteObjectId);
        }

        // check whether this item has been locked for write to disk again.
        public bool CheckDbLocked(Guid siteObjectId)
        {
            return dblock.Contains(siteObjectId);
        }

        #endregion "prevent duplicate DB write back to disk"

        #region "prevent disk write back to DB"

        //public Dictionary<Guid, syncItem> WriteCache = new Dictionary<Guid, syncItem>();

        ////public void AcquireDiskWriteLock(string FullPath, byte[] Bytes)
        ////{
        ////    Log(FullPath);

        ////    var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(FullPath);
        ////    var contenthash = Lib.Security.Hash.ComputeGuid(Bytes);

        ////    if (this.DeletionCache.ContainsKey(pathhash))
        ////    {
        ////        this.DeletionCache.Remove(pathhash);
        ////    }

        ////    if (WriteCache.ContainsKey(pathhash))
        ////    {
        ////        var item = WriteCache[pathhash];
        ////        item.ContentHash = contenthash;
        ////        item.LastModified = DateTime.Now;
        ////    }
        ////    else
        ////    {
        ////        syncItem item = new syncItem() { ContentHash = contenthash };
        ////        WriteCache[pathhash] = item;
        ////    }
        ////}

        //// just write to disk or already has duplicate events.
        //public bool CheckDiskLocked(string FullPath, byte[] bytes)
        //{
        //    Log(FullPath);

        //    var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(FullPath);
        //    var contenthash = Lib.Security.Hash.ComputeGuid(bytes);

        //    if (WriteCache.ContainsKey(pathhash))
        //    {
        //        var item = WriteCache[pathhash];
        //        if (item.ContentHash == contenthash && item.LastModified > DateTime.Now.AddMinutes(-10))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private object _checkAcquireLock = new object();

        ////public bool IsJustWriteToDisk(string fullpath)
        ////{
        ////    Log(fullpath);

        ////    var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);

        ////    if (WriteCache.ContainsKey(pathhash))
        ////    {
        ////        var item = WriteCache[pathhash];
        ////        if (item != null && item.LastModified > DateTime.Now.AddSeconds(-10))
        ////        {
        ////            return true;
        ////        }
        ////    }
        ////    return false;
        ////}

        ////public bool CheckAndAcquireDiskLock(string FullPath, byte[] bytes)
        ////{
        ////    Log(FullPath);

        ////    if (CheckDiskLocked(FullPath, bytes) == false)
        ////    {
        ////        lock (_checkAcquireLock)
        ////        {
        ////            if (CheckDiskLocked(FullPath, bytes) == false)
        ////            {
        ////                AcquireDiskWriteLock(FullPath, bytes);
        ////                return true;
        ////            }
        ////        }
        ////    }
        ////    return false;
        ////}

        ////public bool HasWriteLock(string FullPath)
        ////{
        ////    Log(FullPath);

        ////    var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(FullPath);

        ////    if (this.WriteCache.TryGetValue(pathhash, out syncItem value))
        ////    {
        ////        if (value.LastModified > DateTime.Now.AddMinutes(-10))
        ////        {
        ////            return true;
        ////        }
        ////    }
        ////    return false;

        ////    // return this.WriteCache.ContainsKey(pathhash);
        ////}

        #endregion "prevent disk write back to DB"

        #region "Deletion"

        //public Dictionary<Guid, DateTime> DeletionCache = new Dictionary<Guid, DateTime>();

        //public void AcquireDeletionLock(string FullPath)
        //{
        //    Log(FullPath);

        //    var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(FullPath);

        //    DeletionCache[pathhash] = DateTime.Now;
        //    if (this.WriteCache.ContainsKey(pathhash))
        //    {
        //        this.WriteCache.Remove(pathhash);
        //    }
        //}

        //public bool CheckDeletionLocked(string FullPath)
        //{
        //    Log(FullPath);

        //    var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(FullPath);

        //    if (DeletionCache.TryGetValue(pathhash, out DateTime lastmodified))
        //    {
        //        if (lastmodified > DateTime.Now.AddSeconds(-10))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        #endregion "Deletion"

        #region Addtional to prevent Disk write back to DB.

        private object _lockLocker = new object();

        public Dictionary<Guid, DateTime> FullPathTimeLock = new Dictionary<Guid, DateTime>();

        private HashSet<Guid> absoluteLock = new HashSet<Guid>();

        private Dictionary<Guid, Guid> PathContentHash { get; set; } = new Dictionary<Guid, Guid>();

        public void LockDisk3Seconds(string fullPath)
        {
            Log(fullPath);

            var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(fullPath);
            lock (_lockLocker)
            {
                FullPathTimeLock[pathhash] = DateTime.Now;
            }
        }

        public bool IsDiskLock(string fullPath, int timeElaspe = 4000)
        {
            Log(fullPath);

            var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(fullPath);

            if (absoluteLock.Contains(pathhash))
            {
                return true;
            }

            lock (_lockLocker)
            {
                if (FullPathTimeLock.ContainsKey(pathhash))
                {
                    var item = FullPathTimeLock[pathhash];
                    TimeSpan span = DateTime.Now - item;
                    int ms = (int)span.TotalMilliseconds;

                    return ms < timeElaspe;
                }
                return false;
            }
        }

        public void AbsoluteLock(string fullpath)
        {
            Log(fullpath);

            var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);
            absoluteLock.Add(hash);
        }

        public void ContentHashLock(string fullpath, byte[] binary)
        {
            var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);
            var contenthash = Lib.Security.Hash.ComputeGuid(binary);
            PathContentHash[pathhash] = contenthash;
        }

        public bool IsContentHashLock(string fullpath, byte[] binary)
        {
            var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);
            if (!PathContentHash.ContainsKey(pathhash))
            {
                return false;
            }

            var PreHashValue = PathContentHash[pathhash];
            var contenthash = Lib.Security.Hash.ComputeGuid(binary);

            return PreHashValue == contenthash;
        }

        public void ReleaseAbsoluteLock(string fullpath)
        {
            Log(fullpath);

            var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);
            absoluteLock.Remove(hash);
        }

        #endregion Addtional to prevent Disk write back to DB.

        public void Log(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            logs.Add(memberName + " " + message);
        }

        public List<string> logs = new List<string>();

        public class syncItem
        {
            public DateTime LastModified { get; set; } = DateTime.Now;
            public Guid ContentHash { get; set; }
        }
    }
}