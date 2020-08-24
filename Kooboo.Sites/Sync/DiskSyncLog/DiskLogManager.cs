using Kooboo.Data;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.IO; 

namespace Kooboo.Sites.Sync.DiskSyncLog
{
    public static class DiskLogManager
    {
        private static Kooboo.Data.Log.LogWriter logwrite { get; set; }

        private static string DBPath { get; set; }
        private static Database DB { get; set; }
        static DiskLogManager()
        {
            DBPath = Path.Combine(AppSettings.RootPath, "AppData", "logs", "DiskSync");
            IOHelper.EnsureDirectoryExists(DBPath);
            DB = new Database(DBPath);
            logwrite = new Data.Log.LogWriter("DiskSyncWriteLog"); 
        }

        public static ObjectStore<Guid, DiskFileLog> GetSiteStore(Guid SiteId)
        {
            return DB.GetOrCreateObjectStore<Guid, DiskFileLog>(SiteId.ToString());
        }

        public static void Delete(string fullpath, Guid SiteId)
        {
            var store = GetSiteStore(SiteId);
            var id = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);
            store.delete(id);
            store.Close();

            logwrite.Write("Time: " + DateTime.Now.ToShortTimeString() + " siteid: " + SiteId.ToString() + " del: " + fullpath);  
        }

        public static void Add(string fullpath, Guid SiteId)
        {
            DiskFileLog log = new DiskFileLog();
            log.FullPath = fullpath;
            log.LastModify = DateTime.Now.AddSeconds(2); // this is to give time for disk to write. 
            log.ToDisk = true; 

            var store = GetSiteStore(SiteId);

            var id = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);

            var current = store.get(id);
            if (current == null)
            {
                store.add(log.Id, log);

                logwrite.Write("Time: " + DateTime.Now.ToShortTimeString() + " siteid: " + SiteId.ToString() + " add: " + fullpath);
            }
            else
            {
                current.LastModify = DateTime.Now;
                current.ToDisk = true;
                store.update(current.Id, current);

                logwrite.Write("Time: " + DateTime.Now.ToShortTimeString() + " siteid: " + SiteId.ToString() + " update: " + fullpath);
            }

            store.Close(); 
        }

        public static DiskFileLog Get(string fullpath, Guid SiteId)
        {
            var store = GetSiteStore(SiteId);

            var id = Lib.Security.Hash.ComputeGuidIgnoreCase(fullpath);

            return store.get(id);
        }
         
        public static List<DiskFileLog> All(Guid SiteId)
        {
            var store = GetSiteStore(SiteId);

            return store.Filter.SelectAll();
        }

        //Disk rename....
        public static void RenameFile(string oldFilePath, string newFilePath, Guid SiteId)
        {
            Delete(oldFilePath, SiteId); 
            Add(newFilePath, SiteId);
        }

        public static void RenameFolder(string oldfoler, string newfolder, Guid SiteId)
        {
            var store = GetSiteStore(SiteId);

            var allrecords = store.FullScan(o => o.FullPath.StartsWith(oldfoler, StringComparison.OrdinalIgnoreCase)).SelectAll();

            foreach (var item in allrecords)
            {
                var oldpath = item.FullPath;
                var newpath = oldpath.Replace(oldfoler, newfolder);
                RenameFile(oldpath, newpath, SiteId);
            } 
        }
          
        public static List<DiskChangeEvent> QueryEvents(List<string> fullpath, Guid SiteId)
        {
            List<DiskChangeEvent> result = new List<DiskChangeEvent>();

            var store = GetSiteStore(SiteId);

            Dictionary<Guid, string> dict = new Dictionary<Guid, string>();
            foreach (var item in fullpath)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    var id = Lib.Security.Hash.ComputeGuidIgnoreCase(item);
                    dict[id] = item;
                }
            }

            var all = store.Filter.SelectAll();

            List<DiskFileLog> deleted = all.FindAll(o => !dict.ContainsKey(o.Id));

            List<string> updated = new List<string>();

            foreach (var item in dict)
            {
                if (System.IO.File.Exists(item.Value))
                {
                    var info = new System.IO.FileInfo(item.Value);

                    if (info != null)
                    {
                        var obj = store.get(item.Key);

                        if (obj == null || info.LastWriteTime > obj.LastModify)
                        {
                            updated.Add(item.Value);
                        }
                    }

                }
            }

            foreach (var item in deleted)
            {
                DiskChangeEvent del = new DiskChangeEvent();
                del.ChangeType = DiskChangeType.Deleted;
                del.FullPath = item.FullPath;

                result.Add(del);
            }

            foreach (var item in updated)
            {
                DiskChangeEvent update = new DiskChangeEvent();
                update.ChangeType = DiskChangeType.Updated;
                update.FullPath = item;

                result.Add(update);
            }

            foreach (var item in deleted)
            {
                store.delete(item.Id); 
            }

            return result;
        }

    }
}
