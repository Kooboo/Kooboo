//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync.Disk;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Sync
{
 public static   class DiskSyncHelper
    { 
        static DiskSyncHelper()
        {
            _locker = new object(); 
        }

        private static object _locker { get; set; }
           
        public static string SyncToDisk(SiteDb SiteDb, ISiteObject Value, ChangeType ChangeType, string StoreName)
        {
            var manager =  new  SyncManager(SiteDb.WebSite.Id);
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
                    var manager = new SyncManager(SiteDb.WebSite.Id); 
                    IOHelper.EnsureFileDirectoryExists(NewFullPath); 

                    var allbytes = Lib.Helper.IOHelper.ReadAllBytes(OldFullPath); 

                    manager.WriteBytes(NewFullPath, allbytes);
                    manager.Delete(OldFullPath);  
                }
            }
        }
    }
}
