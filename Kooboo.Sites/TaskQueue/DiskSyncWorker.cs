//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync.Disk;
using System;
using System.Linq;

namespace Kooboo.Sites.TaskQueue
{
    public class DiskSyncWorker : IBackgroundWorker
    {
        public static object _locker = new object();

        public int Interval
        {
            get
            {
                return 30;
            }
        }

        public DateTime LastExecute
        {
            get; set;
        }


        public void Execute()
        {
            if (Data.AppSettings.IsOnlineServer)
            {
                return;  
            }

           
            var allwebsites = Kooboo.Data.GlobalDb.WebSites.AllSites.Values.ToList();
            for (int i = 0; i < allwebsites.Count; i++)
            {
                var item = allwebsites[i];
                if (item.EnableDiskSync)
                {
                    Execute(item.SiteDb());
                }
            } 
        }

        public void Execute(SiteDb sitedb)
        {
            lock (_locker)
            {
                var manager = new SyncManager(sitedb.WebSite.Id);
                var items = sitedb.Synchronization.GetPushItems(sitedb.Synchronization.DiskSyncSettingId);
                foreach (var item in items)
                {
                    var repo = sitedb.GetRepository(item.StoreName);

                    if (repo == null)
                    {
                        return;
                    }
                    var siteobject = repo.GetByLog(item);

                    if (siteobject == null)
                    {
                        return;
                    }

                    ChangeType changetype = ChangeType.Update;
                    if (item.EditType == IndexedDB.EditType.Delete)
                    {
                        changetype = ChangeType.Delete;
                    }
                    else if (item.EditType == IndexedDB.EditType.Add)
                    {
                        changetype = ChangeType.Add;
                    }

                    manager.SyncToDisk(sitedb, siteobject, changetype, item.StoreName);

                    // Ensure Double check..... cost performance for integrity. 
                    sitedb.Synchronization.AddOrUpdate(new Models.Synchronization() { SyncSettingId = sitedb.Synchronization.DiskSyncSettingId, ObjectId = siteobject.Id, Version = item.Id, In = false, StoreName = repo.StoreName });
                }
            }
        }

    }
}
