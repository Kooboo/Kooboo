//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kooboo.Sites.Sync
{
    public static class DiskSyncFolderWatcher
    {
        public static Dictionary<Guid, System.IO.FileSystemWatcher> watchers = new Dictionary<Guid, System.IO.FileSystemWatcher>();

        public static HashSet<Guid> PathHash = new HashSet<Guid>();

        private static object _lockobject = new object();

        public static void StartDiskWatcher(Data.Models.WebSite webSite)
        {
            if (!webSite.EnableDiskSync)
            { return; }

            if (!watchers.ContainsKey(webSite.Id))
            {
                lock (_lockobject)
                {
                    if (!watchers.ContainsKey(webSite.Id))
                    {
                        var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(webSite.DiskSyncFolder);

                        if (PathHash.Contains(hash))
                        {
                            return;
                        }

                        try
                        {
                            IOHelper.EnsureDirectoryExists(webSite.DiskSyncFolder);
                        }
                        catch (Exception)
                        {
                            return;
                        }

                        FileSystemWatcher watcher = new System.IO.FileSystemWatcher(webSite.DiskSyncFolder)
                        {
                            IncludeSubdirectories = true,
                            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite |
                                           NotifyFilters.DirectoryName
                        };



                        DiskSyncManager manager = DiskSyncHelper.GetSyncManager(webSite.Id);

                        watcher.Changed += new FileSystemEventHandler(manager.OnChanged);
                        watcher.Created += new FileSystemEventHandler(manager.OnChanged);
                        watcher.Deleted += new FileSystemEventHandler(manager.OnChanged);
                        watcher.Renamed += new RenamedEventHandler(manager.OnRenamed);

                        watcher.EnableRaisingEvents = true;

                        watchers[webSite.Id] = watcher;

                        PathHash.Add(hash);
                    }
                }
            }
        }

        public static void StopDiskWatcher(Data.Models.WebSite website)
        {
            lock (_lockobject)
            {
                if (watchers.ContainsKey(website.Id))
                {
                    var watcher = watchers[website.Id];
                    watcher.EnableRaisingEvents = false;
                    watchers.Remove(website.Id);
                    watcher.Dispose();
                    DiskSyncHelper.RemoveDiskSyncManager(website.Id);

                    var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(website.DiskSyncFolder);

                    PathHash.Remove(hash);
                }
            }
        }
    }
}