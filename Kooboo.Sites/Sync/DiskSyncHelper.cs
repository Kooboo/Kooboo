//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Sync
{
    public static class DiskSyncHelper
    {
        static DiskSyncHelper()
        {
            _locker = new object();
            ManagerCache = new Dictionary<Guid, DiskSyncManager>();
        }

        private static object _locker { get; set; }

        private static Dictionary<Guid, DiskSyncManager> ManagerCache { get; set; }

        public static DiskSyncManager GetSyncManager(Guid webSiteId)
        {
            if (!ManagerCache.ContainsKey(webSiteId))
            {
                lock (_locker)
                {
                    if (!ManagerCache.ContainsKey(webSiteId))
                    {
                        var newmanager = new DiskSyncManager(webSiteId);
                        ManagerCache[webSiteId] = newmanager;
                    }
                }
            }
            return ManagerCache[webSiteId];
        }

        public static void RemoveDiskSyncManager(Guid webSiteId)
        {
            lock (_locker)
            {
                ManagerCache.Remove(webSiteId);
            }
        }

        public static string SyncToDisk(SiteDb siteDb, ISiteObject value, ChangeType changeType, string storeName)
        {
            var manager = GetSyncManager(siteDb.WebSite.Id);
            return manager.SyncToDisk(siteDb, value, changeType, storeName);
        }

        public static void ChangeRoute(SiteDb siteDb, string oldRelative, string newRelative)
        {
            if (siteDb.WebSite.EnableDiskSync)
            {
                var oldFullPath = DiskPathService.GetFullDiskPath(siteDb.WebSite, oldRelative);

                var newFullPath = DiskPathService.GetFullDiskPath(siteDb.WebSite, newRelative);

                if (System.IO.File.Exists(oldFullPath) && !System.IO.File.Exists(newFullPath))
                {
                    var manager = GetSyncManager(siteDb.WebSite.Id);
                    IOHelper.EnsureFileDirectoryExists(newFullPath);

                    var allbytes = Lib.Helper.IOHelper.ReadAllBytes(oldFullPath);

                    manager.SyncMediator.AbsoluteLock(newFullPath);

                    manager.WriteBytes(newFullPath, allbytes);

                    manager.SyncMediator.LockDisk3Seconds(newFullPath);
                    manager.SyncMediator.ReleaseAbsoluteLock(newFullPath);

                    manager.SyncMediator.AbsoluteLock(oldFullPath);

                    System.IO.File.Delete(oldFullPath);

                    manager.SyncMediator.LockDisk3Seconds(oldFullPath);
                    manager.SyncMediator.ReleaseAbsoluteLock(oldFullPath);
                }
            }
        }
    }
}