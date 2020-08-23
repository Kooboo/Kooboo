//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync.DiskSyncLog;
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

        public DiskSyncManager(Guid WebSiteId)
        {
            this.WebSiteId = WebSiteId;
            this.SyncMediator = new SyncMediator();
            _queuelocker = new object();
        }

        private int MaxThread = 10;
        private int CurrentThreadCount = 0;

        private Guid WebSiteId { get; set; }

        private bool CanAccept
        {
            get
            { return this.CurrentThreadCount < MaxThread; }
        }

        public List<DiskChangeEvent> ChangeTasks { get; set; } = new List<DiskChangeEvent>();

        public bool AddTask(DiskChangeEvent ChangeItem)
        {
            lock (_queuelocker)
            {

                if (ChangeItem.ChangeType == DiskChangeType.Created && this.SyncMediator.IsDiskLock(ChangeItem.FullPath, int.MaxValue))
                {
                    return false;
                }
                var SameItems = this.ChangeTasks.FindAll(o => o.FullPath == ChangeItem.FullPath).ToList();

                if (SameItems == null || SameItems.Count == 0)
                {
                    this.ChangeTasks.Add(ChangeItem);
                    return true;
                }
                else
                {
                    if (ChangeItem.ChangeType == DiskChangeType.Deleted)
                    {
                        foreach (var item in SameItems)
                        {
                            if (!item.Peeked && item.ChangeType != DiskChangeType.Deleted)
                            {
                                this.ChangeTasks.Remove(item);
                            }
                        }

                        if (SameItems.Find(o => o.ChangeType == DiskChangeType.Deleted) == null)
                        {
                            this.ChangeTasks.Add(ChangeItem);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (ChangeItem.ChangeType == DiskChangeType.Rename)
                    {
                        var item = SameItems.Find(o => o.ChangeType == DiskChangeType.Rename && o.OldFullPath == ChangeItem.OldFullPath && !o.Peeked);

                        if (item != null)
                        {
                            item.FullPath = ChangeItem.FullPath;
                            return false;
                        }
                        else
                        {
                            this.ChangeTasks.Add(ChangeItem);
                            return true;
                        }
                    }
                    else
                    {
                        // if there is an update item to be executed, do nothing. 
                        if (SameItems.Find(o => (o.ChangeType == DiskChangeType.Created || o.ChangeType == DiskChangeType.Updated)) != null)
                        {
                            return false;
                        }

                        else
                        {
                            this.ChangeTasks.Add(ChangeItem);
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
                // ProcessFromDisk(); 
            }
        }
        internal void ProcessFromDisk()
        {
            if (this.CanAccept)
            {
                Interlocked.Increment(ref this.CurrentThreadCount);
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

                                DiskSyncLog.DiskLogManager.RenameFolder(task.OldFullPath, task.FullPath, sitedb.WebSite.Id);
                            }
                            else
                            {
                                SyncService.DiskFileRename(sitedb, task.OldFullPath, task.FullPath);

                                DiskSyncLog.DiskLogManager.RenameFile(task.OldFullPath, task.FullPath, sitedb.WebSite.Id);
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
                                DiskSyncLog.DiskLogManager.Delete(task.FullPath, sitedb.WebSite.Id);
                            }
                        }
                        else
                        {
                            this.SyncToDb(task.FullPath, sitedb, null);
                            DiskSyncLog.DiskLogManager.Add(task.FullPath, sitedb.WebSite.Id);
                        }

                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;
                    }

                    this.RemoveTask(task);
                }
                Interlocked.Decrement(ref this.CurrentThreadCount);

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
                DiskChangeEvent theevent = new DiskChangeEvent();
                theevent.OldFullPath = e.OldFullPath;
                theevent.FullPath = e.FullPath;
                theevent.ChangeType = DiskChangeType.Rename;

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

            DiskChangeEvent theevent = new DiskChangeEvent();
            theevent.FullPath = e.FullPath;

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                theevent.ChangeType = DiskChangeType.Updated;
            }
            else if (e.ChangeType == WatcherChangeTypes.Created)
            {
                theevent.ChangeType = DiskChangeType.Created;
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                theevent.ChangeType = DiskChangeType.Deleted;
            }
            else if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                throw new Exception("should fire rename event");
            }

            if (this.AddTask(theevent))
            {
                startNewFromDisk();
            }
        }
        public void DeleteFromDb(string DiskFullPath, SiteDb sitedb)
        {

            var NonRoutable = DiskPathService.GetNonRoutableObject(DiskFullPath);

            if (NonRoutable != null)
            {
                var repo = sitedb.GetRepository(NonRoutable.StoreName);
                string name = string.IsNullOrWhiteSpace(NonRoutable.Name) ? NonRoutable.Id.ToString() : NonRoutable.Name;
                var result = repo.GetByNameOrId(name) as ISiteObject;
                if (result != null)
                {
                    this.SyncMediator.AcquireDbLock(result.Id);
                    repo.Delete(result.Id);
                    this.SyncMediator.ReleaseDbLock(result.Id);
                }
            }
            else
            {
                var RelativeUrl = DiskPathService.GetRelativeUrl(sitedb.WebSite, DiskFullPath);
                var route = sitedb.Routes.GetByUrl(RelativeUrl);
                if (route != null)
                {
                    var repo = sitedb.GetRepository(route.DestinationConstType);
                    if (repo != null)
                    {
                        var result = repo.Get(route.objectId) as ISiteObject;
                        if (result != null)
                        {
                            this.SyncMediator.AcquireDbLock(result.Id);
                            repo.Delete(result.Id);
                            this.SyncMediator.ReleaseDbLock(result.Id);
                        }
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

            if (!parts[0].Contains("."))
            {
                return false;
            }
            return true;
        }

        public void SyncToDb(string FullPath, SiteDb SiteDb, byte[] diskbytes = null, bool logSync = true)
        {

            if (diskbytes == null)
            {
                diskbytes = this.ReadAllBytes(FullPath);
            }
            if (diskbytes == null)
            {
                return;
            }

            if (this.SyncMediator.IsContentHashLock(FullPath, diskbytes))
            {
                return;
            }
     
            string OldRelativeUrl = null;
            string RelativeUrl = null;

            IRepository repo = null;
            ISiteObject result = null;
            Routing.Route route = null;
            string NameFromFile = null;
            string extension = UrlHelper.FileExtension(FullPath);
            if (!string.IsNullOrEmpty(extension) && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.ToLower();
            }

            var NonRoutable = DiskPathService.GetNonRoutableObject(FullPath);

            if (NonRoutable != null)
            {
                repo = SiteDb.GetRepository(NonRoutable.StoreName);
                NameFromFile = NonRoutable.Name;
                string name = string.IsNullOrWhiteSpace(NonRoutable.Name) ? NonRoutable.Id.ToString() : NonRoutable.Name;
                if (!string.IsNullOrEmpty(NonRoutable.Extension))
                {
                    extension = NonRoutable.Extension.ToLower();
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
                OldRelativeUrl = DiskPathService.GetRelativeUrl(SiteDb.WebSite, FullPath);
                RelativeUrl = Kooboo.Sites.Helper.RouteHelper.ToValidRoute(OldRelativeUrl);

                route = SiteDb.Routes.GetByUrl(RelativeUrl);
                if (route != null)
                {
                    repo = SiteDb.GetRepository(route.DestinationConstType);
                    result = repo.Get(route.objectId) as ISiteObject;
                }
                else
                {
                    var ModelType = Service.ConstTypeService.GetModelTypeByUrl(RelativeUrl);
                    if (ModelType == null) { return; }
                    repo = SiteDb.GetRepository(ModelType);
                }
                NameFromFile = UrlHelper.FileName(RelativeUrl);
            }

            if (result == null)
            {
                result = Activator.CreateInstance(repo.ModelType) as ISiteObject;
            }

            if (!DiskObjectConverter.FromBytes(ref result, diskbytes))
            {
                return;
            }

            if (result is IExtensionable)
            {
                var extensionfile = result as IExtensionable;
                extensionfile.Extension = extension;
            }

            if (string.IsNullOrEmpty(result.Name))
            {
                result.Name = Lib.Helper.StringHelper.ToValidFileName(NameFromFile);
            }

            #region "Routing"

            if (!string.IsNullOrEmpty(RelativeUrl))
            {
                SiteDb.Routes.AddOrUpdate(RelativeUrl, result as SiteObject);
            }
            else
            {
                // # Rule1, only the API is different...
                if (result is Kooboo.Sites.Models.Code)
                {
                    var code = result as Code; 
                    if (code.CodeType == CodeType.Api)
                    {
                        bool shouldUpdateCodeRouteText = false; 
                      
                        var diskroute = DiskObjectConverter.GetRouteFromCodeBytes(diskbytes); 
                        if (string.IsNullOrWhiteSpace(diskroute))
                        {
                            // # Rule2, Api must have a route defined, otherwise it is a new api. 
                            var newroute = DiskObjectConverter.GetNewRoute(SiteDb, code.Name);
                            SiteDb.Routes.AddOrUpdate(newroute, code);
                            shouldUpdateCodeRouteText = true; 
                        }
                        else
                        {
                            // # Rule 3, Check if this is its own route, or someelse routes. 
                            // Own rule, do nothing. 
                            var coderoute = SiteDb.Routes.Get(diskroute);
                            if (coderoute == null)
                            {
                                //#Rule 4, If route does not exists yet. Add and end. 
                                SiteDb.Routes.AddOrUpdate(diskroute, code);  
                            }
                            else
                            { 
                                if (coderoute.objectId != default(Guid) && coderoute.objectId != code.Id)
                                {
                                    // #Rule 5, This is route for others... get a new route.
                                    var newcoderoute = DiskObjectConverter.GetNewRoute(SiteDb, diskroute);
                                    SiteDb.Routes.AddOrUpdate(newcoderoute, code);
                                    shouldUpdateCodeRouteText = true; 
                                } 
                            }

                        }

                        if (shouldUpdateCodeRouteText)
                        {
                            this.SyncToDisk(SiteDb, code, ChangeType.Update, SiteDb.Code.StoreName); 
                        }
                        
                    }

                }
            }

            #endregion

            if (!isSameName(result.Name, NameFromFile, extension) || OldRelativeUrl != RelativeUrl)
            {
                if (File.Exists(FullPath))
                {
                    this.SyncMediator.AbsoluteLock(FullPath);

                    File.Delete(FullPath);

                    this.SyncMediator.LockDisk3Seconds(FullPath);
                    this.SyncMediator.ReleaseAbsoluteLock(FullPath);
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

        private bool isSameName(string x, string y, string extension)
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
         
        public string SyncToDisk(SiteDb SiteDb, ISiteObject Value, ChangeType ChangeType, string StoreName)
        {
            string diskpath = null;

            if (Attributes.AttributeHelper.IsDiskable(Value) && !IsEmbedded(Value) && !string.IsNullOrEmpty(StoreName))
            {
                if (!this.SyncMediator.CheckDbLocked(Value.Id))
                {
                    var value = Value as ISiteObject;
                    string relativeurl = DiskPathService.GetObjectRelativeUrlForDiskSync(value, SiteDb, StoreName);

                    if (!string.IsNullOrEmpty(relativeurl))
                    {
                        string fullpath = DiskPathService.GetFullDiskPath(SiteDb.WebSite, relativeurl);

                        if (ChangeType == ChangeType.Delete)
                        {
                            if (File.Exists(fullpath))
                            {
                                diskpath = fullpath;
                                 
                                this.SyncMediator.AbsoluteLock(fullpath);
                                File.Delete(fullpath); 

                                this.SyncMediator.LockDisk3Seconds(fullpath);
                                this.SyncMediator.ReleaseAbsoluteLock(fullpath);

                                DiskSyncLog.DiskLogManager.Delete(fullpath, SiteDb.Id);

                            }
                        }

                        else
                        {
                            var coreobject = value as ICoreObject;

                            if (coreobject != null)
                            {
                                bool hasPast = false;

                                var logs = SiteDb.Synchronization.Query.Where(o => o.SyncSettingId == SiteDb.Synchronization.DiskSyncSettingId && o.ObjectId == value.Id).SelectAll();

                                foreach (var item in logs)
                                {
                                    if (StringHelper.IsSameValue(StoreName, item.StoreName))
                                    {
                                        if (item.Version >= coreobject.Version)
                                        {
                                            hasPast = true;
                                        }
                                    }
                                }

                                if (!hasPast)
                                {
                                    var contentbytes = DiskObjectConverter.ToBytes(SiteDb, value);

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
                                        this.SyncMediator.AbsoluteLock(fullpath);
                                        this.SyncMediator.ContentHashLock(fullpath, contentbytes);

                                        this.WriteBytes(fullpath, contentbytes);

                                        this.SyncMediator.LockDisk3Seconds(fullpath);
                                        this.SyncMediator.ReleaseAbsoluteLock(fullpath);

                                        diskpath = fullpath;
                                    }

                                    DiskSyncLog.DiskLogManager.Add(fullpath, SiteDb.Id, true);

                                }
                            }

                        }
                    }
                }

            }

            var core = Value as ICoreObject;

            if (core != null)
            {
                SiteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = SiteDb.Synchronization.DiskSyncSettingId, ObjectId = Value.Id, Version = core.Version, StoreName = StoreName });
            }

            return diskpath;
        }

        public void InitSync()
        {
            this.InitSyncToDisk();
            this.InitSyncToDB(); 
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

        public void InitSyncToDB()
        {
            var website = Kooboo.Data.GlobalDb.WebSites.Get(this.WebSiteId);
            if (website != null && website.EnableDiskSync)
            {
                var basefolder = website.DiskSyncFolder;

                var allfiles = System.IO.Directory.GetFiles(basefolder, "*.*", SearchOption.AllDirectories);

                if (allfiles != null && allfiles.Any())
                {
                    var events = DiskSyncLog.DiskLogManager.QueryEvents(allfiles.ToList(), this.WebSiteId);

                    foreach (var item in events)
                    {
                        this.AddTask(item);
                    }

                    startNewFromDisk();
                }

            }

        }
         

        private bool IsEmbedded(ISiteObject Value)
        {
            if (Value is IEmbeddable)
            {
                IEmbeddable embedded = Value as IEmbeddable;
                return embedded.IsEmbedded;
            }
            return false;
        }

        private object _IOLocker = new object();

        private byte[] ReadAllBytes(string FilePath)
        {
            lock (_IOLocker)
            {
                if (File.Exists(FilePath))
                {
                    int i = 0;
                    System.Threading.Thread.Sleep(10);  //TODO:  this is very strange action, otherwise, file will be being used.  
                    while (i < 10)
                    {
                        try
                        {
                            if (File.Exists(FilePath))
                            {
                                var bytes = File.ReadAllBytes(FilePath);
                                return bytes;
                            }
                        }
                        catch (Exception)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        i += 1;
                    }
                }
                return null;
            }
        }

        internal void WriteBytes(string FullPath, byte[] Value)
        {
            if (Value == null)
            {
                Value = new byte[0];
            }
            lock (_IOLocker)
            {
                IOHelper.EnsureFileDirectoryExists(FullPath);

                int i = 0;
                while (i < 10)
                {
                    try
                    {

                        System.IO.FileStream stream = new FileStream(FullPath, FileMode.Create);

                        stream.Write(Value, 0, Value.Length);

                        stream.Close();
                        stream.Dispose();

                    }
                    catch (Exception)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    i = i + 1;
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
        HashSet<Guid> dblock = new HashSet<Guid>();

        #region "prevent duplicate DB write back to disk"  

        public void AcquireDbLock(Guid SiteObjectId)
        {
            dblock.Add(SiteObjectId);
        }

        public void ReleaseDbLock(Guid SiteObjectId)
        {
            dblock.Remove(SiteObjectId);
        }

        // check whether this item has been locked for write to disk again. 
        public bool CheckDbLocked(Guid SiteObjectId)
        {
            return dblock.Contains(SiteObjectId);
        }

        #endregion

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

        #endregion

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

        #endregion



        #region Addtional to prevent Disk write back to DB. 

        private object _lockLocker = new object();

        public Dictionary<Guid, DateTime> FullPathTimeLock = new Dictionary<Guid, DateTime>();

        private HashSet<Guid> absoluteLock = new HashSet<Guid>();

        private Dictionary<Guid, Guid> PathContentHash { get; set; } = new Dictionary<Guid, Guid>();

        public void LockDisk3Seconds(string FullPath)
        {
            Log(FullPath);

            var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(FullPath);
            lock (_lockLocker)
            {
                FullPathTimeLock[pathhash] = DateTime.Now;
            }
        }

        public bool IsDiskLock(string FullPath, int TimeElaspe = 4000)
        {
            Log(FullPath);

            var pathhash = Lib.Security.Hash.ComputeGuidIgnoreCase(FullPath);

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

                    return ms < TimeElaspe;
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

        #endregion


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

