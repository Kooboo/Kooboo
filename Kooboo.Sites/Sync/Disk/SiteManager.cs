using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kooboo.Sites.Sync.Disk
{
    public static class SiteManager
    {
        private static Dictionary<Guid, DateTime> LastCheck { get; set; } = new Dictionary<Guid, DateTime>();

        public static DateTime GetLastCheck(Guid SiteId)
        {
            if (!LastCheck.ContainsKey(SiteId))
            {
                return default(DateTime);
            }
            else
            {
                return LastCheck[SiteId];
            }
        }

        public static void SetLastCheck(Guid SiteId, DateTime time)
        {
            LastCheck[SiteId] = time;
        }

        public static void CheckDisk()
        {
            var list = GetSitesToCheck();
            foreach (var item in list)
            {
                SiteChecker.Check(item);
            }
        }

        public static List<WebSite> GetSitesToCheck()
        {
            List<WebSite> toCheck = new List<WebSite>();

            var list = Kooboo.Data.GlobalDb.WebSites.AllSites.Values.ToList().FindAll(o => o.EnableDiskSync);

            foreach (var item in list)
            {
                var lastcheck = GetLastCheck(item.Id);
                if (lastcheck == default(DateTime))
                {
                    toCheck.Add(item);
                }
                else
                {
                    if (HasChange(item.DiskSyncFolder, lastcheck))
                    {
                        toCheck.Add(item);
                    }
                }
            }

            foreach (var item in toCheck)
            {
                SetLastCheck(item.Id, DateTime.Now);
            }

            return toCheck;
        }

        // check if there is any change of this folder since last check. 
        public static bool HasChange(string fulldir, DateTime CompareTime)
        {
            if (!System.IO.Directory.Exists(fulldir))
            {
                return false;
            }

            var dir = new System.IO.DirectoryInfo(fulldir);
            var files = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            foreach (var item in files)
            {
                if (item.LastWriteTime > CompareTime)
                {
                    return true;
                }
            }

            return false;
            // return HasNewer(dir, CompareTime);
        }

        private static bool HasNewer(System.IO.DirectoryInfo dir, DateTime CompareTime)
        {
            if (dir.LastWriteTime > CompareTime)
            {
                return true;
            }
            else
            {
                var subs = dir.GetDirectories();
                foreach (var item in subs)
                {
                    if (HasNewer(item, CompareTime))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
