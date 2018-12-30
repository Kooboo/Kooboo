//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Extensions
{
    public static class JobExtension
    {

        /// <summary>
        /// list all the schdule tasks that belongs to this website. 
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="WebSiteId"></param>
        /// <returns></returns>
        public static List<ScheduleItem<Job>> GetByWebSiteId(this Schedule<Job> jobs, Guid WebSiteId)
        {
            List<ScheduleItem<Job>> sitejobs = new List<ScheduleItem<Job>>();

            // This to be improved, should use a kind of foreach collection. 
            var alljobs = jobs.Read(9999);

            foreach (var item in alljobs)
            {
                if (item != null && item.Item.WebSiteId == WebSiteId)
                {
                    sitejobs.Add(item);
                }
            }

            return sitejobs;
        }

        public static List<RepeatItem<Job>> GetByWebSiteId(this  RepeatTask<Job> jobs, Guid WebSiteId)
        {
            List<RepeatItem<Job>> sitejobs = new List<RepeatItem<Job>>();

            foreach (var item in jobs.GetItems())
            {
                if (item !=null && item.Item !=null && item.Item.WebSiteId == WebSiteId)
                {
                    sitejobs.Add(item);
                }
            }

            return sitejobs;
        }

        /// <summary>
        /// Get the log infomation by website id, order by descending. 
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public static List<JobLog> GetByWebSiteId(this Sequence<JobLog> logs, Guid WebSiteId, int take, int skip = 0)
        {
            int taken = 0;
            int skipped = 0;

            List<JobLog> loglist = new List<JobLog>();

            foreach (var item in logs.GetCollection(false))
            {
                if (item.WebSiteId == WebSiteId)
                {
                    if (skipped < skip)
                    {
                        skipped += 1;
                        continue; 
                    }

                    if (taken >= take)
                    {
                        return loglist; 
                    }

                    loglist.Add(item);

                    taken += 1;
                    if (taken >= take)
                    {
                        return loglist;
                    }
                }
            }

            return loglist; 


        }

        public static List<JobLog> GetByWebSiteId(this Sequence<JobLog> logs, Guid WebSiteId, bool isSuccess, int take, int skip = 0)
        {

            int taken = 0;
            int skipped = 0;

            List<JobLog> loglist = new List<JobLog>();

            foreach (var item in logs.GetCollection(false))
            {
                if (item.WebSiteId == WebSiteId && item.Success == isSuccess)
                {
                    if (skipped < skip)
                    {
                        skipped += 1;
                        continue;
                    }

                    if (taken >= take)
                    {
                        return loglist;
                    }

                    loglist.Add(item);

                    taken += 1;
                    if (taken >= take)
                    {
                        return loglist;
                    }
                }
            }

            return loglist; 

        }

    }
}
