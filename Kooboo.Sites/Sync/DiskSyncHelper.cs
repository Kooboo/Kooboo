//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Sync
{
 public static   class DiskSyncHelper
    { 
        static DiskSyncHelper()
        {
            _locker = new object();
            ManagerCache = new Dictionary<Guid, DiskSyncManager>(); 
        }

        private static object _locker { get; set; }

        private static Dictionary<Guid, DiskSyncManager> ManagerCache { get; set; }
        
        public static DiskSyncManager GetSyncManager(Guid WebSiteId)
        {
            if (!ManagerCache.ContainsKey(WebSiteId))
            {
                lock(_locker)
                {
                    if (!ManagerCache.ContainsKey(WebSiteId))
                    {
                        var newmanager = new DiskSyncManager(WebSiteId);
                        ManagerCache[WebSiteId] = newmanager; 
                    }
                }
            } 
            return ManagerCache[WebSiteId]; 
        }

        public static void RemoveDiskSyncManager(Guid WebSiteId)
        {
            lock(_locker)
            {
                ManagerCache.Remove(WebSiteId); 
            }
        }

        public static string SyncToDisk(SiteDb SiteDb, ISiteObject Value, ChangeType ChangeType, string StoreName)
        {
            var manager = GetSyncManager(SiteDb.WebSite.Id);
            return  manager.SyncToDisk(SiteDb, Value, ChangeType, StoreName); 
        } 

        public static void ChangeRoute(SiteDb SiteDb, string OldRelative, string NewRelative)
        {
            if (SiteDb.WebSite.EnableDiskSync)
            {
                var OldFullPath = DiskPathService.GetFullDiskPath(SiteDb.WebSite, OldRelative);

                var NewFullPath = DiskPathService.GetFullDiskPath(SiteDb.WebSite, NewRelative);
                
                if (System.IO.File.Exists(OldFullPath) && !System.IO.File.Exists(NewFullPath))
                { 
                    var manager = GetSyncManager(SiteDb.WebSite.Id); 
                    IOHelper.EnsureFileDirectoryExists(NewFullPath);


                    var allbytes = Lib.Helper.IOHelper.ReadAllBytes(OldFullPath);

                    manager.SyncMediator.AbsoluteLock(NewFullPath);

                    manager.WriteBytes(NewFullPath, allbytes);

                    manager.SyncMediator.LockDisk3Seconds(NewFullPath);
                    manager.SyncMediator.ReleaseAbsoluteLock(NewFullPath);

                                                        

                    manager.SyncMediator.AbsoluteLock(OldFullPath);

                    System.IO.File.Delete(OldFullPath);

                    manager.SyncMediator.LockDisk3Seconds(OldFullPath);
                    manager.SyncMediator.ReleaseAbsoluteLock(OldFullPath);    
                                                     
                }
            }
        }
    }
}
