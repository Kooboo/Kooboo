using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Sync.Disk
{
    public static class SiteManager
    {
        private static Dictionary<Guid, CheckResult> LastCheck { get; set; } = new Dictionary<Guid, CheckResult>();

        public static CheckResult GetLastCheck(Guid SiteId)
        {
            if (!LastCheck.ContainsKey(SiteId))
            {
                return new CheckResult() { AccessTime = default(DateTime), FileCount = 0 };
            }
            else
            {
                return LastCheck[SiteId];
            }
        }

        public static void SetLastCheck(Guid SiteId, DateTime accesstime, int filecount)
        {
            CheckResult result;
            if (LastCheck.ContainsKey(SiteId))
            {
                result = LastCheck[SiteId];
                result.AccessTime = accesstime;
                result.FileCount = filecount;
            }
            else
            {
                result = new CheckResult();
                result.AccessTime = accesstime;
                result.FileCount = filecount;
                LastCheck[SiteId] = result;
            }
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
                if (!System.IO.Directory.Exists(item.DiskSyncFolder))
                {
                    continue;
                }

                var dir = new System.IO.DirectoryInfo(item.DiskSyncFolder);
                var files = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                var filecount = files.Count();

                var lastcheck = GetLastCheck(item.Id);
                var lastchecktime = lastcheck.AccessTime;

                if (filecount != lastcheck.FileCount || files.Any(o => o.LastWriteTime > lastchecktime))
                {
                    toCheck.Add(item);
                    SetLastCheck(item.Id, DateTime.Now, filecount);
                } 
            }
            return toCheck;
        }
    }

    public class CheckResult
    {
        public DateTime AccessTime { get; set; }

        public int FileCount { get; set; }
    }
}
