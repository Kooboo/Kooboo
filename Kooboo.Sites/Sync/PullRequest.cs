//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.Sync
{
    public static class PullRequest
    {
        // pull response to local request.
        public static Sync.SyncObject PullNext(SiteDb siteDb, long currentLogId)
        {
            var log = GetNextLog(siteDb, currentLogId);
            if (log == null)
            { return null; }

            //if (HasFurtherLog(SiteDb, log))
            //{
            //    return PullNext(SiteDb, log.Id);
            //}
            //else
            //{
            return Kooboo.Sites.Sync.SyncService.Prepare(siteDb, log);
            // }
        }

        public static LogEntry GetNextLog(SiteDb siteDb, long currentLog)
        {
            for (long i = currentLog + 1; i < currentLog + 10; i++)
            {
                var log = siteDb.Log.Get(i);
                if (log != null)
                {
                    return log;
                }
            }
            return siteDb.Log.Store.Where(o => o.Id > currentLog).OrderByAscending(o => o.Id).FirstOrDefault();
        }

        public static bool HasFurtherLog(SiteDb siteDb, LogEntry log)
        {
            var nextlog = siteDb.Log.Store.Where(o => o.Id > log.Id && o.KeyHash == log.KeyHash).OrderByDescending().FirstOrDefault();

            return nextlog != null;
        }
    }
}