using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Sync.Disk
{
   public static class SiteChecker
    { 
        public static void Check(WebSite site)
        {
            if (site == null || !site.EnableDiskSync)
            {
                return; 
            }

            var basefolder = site.DiskSyncFolder;

            var allfiles = System.IO.Directory.GetFiles(basefolder, "*.*", SearchOption.AllDirectories);

            List<string> filelist = new List<string>();
            if (allfiles != null)
            {
                filelist = allfiles.ToList();
            }

            var events = DiskSyncLog.DiskLogManager.QueryEvents(filelist, site.Id);

            if (events != null && events.Any())
            {
                var syncmanager = new SyncManager(site.Id); 

                foreach (var item in events)
                {
                    syncmanager.ProcessDiskEvent(site, item);  
                } 
            }
        } 

    }
}
