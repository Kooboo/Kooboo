//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Repository;
using System;
using System.Linq;

namespace Kooboo.Sites.Sync
{
    public static class PullRequest
    {
        // pull response to local request.
        public static Sync.SyncObject PullNext(SiteDb SiteDb, long CurrentLogId)
        {
            var log = GetNextLog(SiteDb, CurrentLogId); 
            if (log == null)
            { return null; }

            //if (HasFurtherLog(SiteDb, log))
            //{
            //    return PullNext(SiteDb, log.Id);
            //}
            //else
            //{
                return Kooboo.Sites.Sync.SyncService.Prepare(SiteDb, log);  
           // }
        }

        public static LogEntry GetNextLog(SiteDb SiteDb, long CurrentLog)
        {
            for (long i = CurrentLog + 1; i < CurrentLog + 10; i++)
            {
                var log = SiteDb.Log.Get(i);
                if (log != null)
                {
                    return log;
                }
            }
            return SiteDb.Log.Store.Where(o => o.Id > CurrentLog).OrderByAscending(o => o.Id).FirstOrDefault(); 
        }

        public static bool HasFurtherLog(SiteDb SiteDb, LogEntry log)
        {
            var nextlog = SiteDb.Log.Store.Where(o => o.Id > log.Id && o.KeyHash == log.KeyHash).OrderByDescending().FirstOrDefault();

            return nextlog != null;   

        } 
    } 
}
