//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.TaskQueue
{
    public static class QueueManager
    {
        private static object _locker = new object();

        private static Dictionary<Guid, int> FailTimes = new Dictionary<Guid, int>();
        public static void Add(object queuevalue, Guid WebSiteId = default(Guid))
        {
            Kooboo.Data.Models.Queue value = new Data.Models.Queue();
            value.TaskModelType = queuevalue.GetType().FullName;
            value.JsonModel = Lib.Helper.JsonHelper.Serialize(queuevalue);
            value.WebSiteId = WebSiteId;
            Kooboo.Data.GlobalDb.TaskQueue.AddOrUpdate(value);
        }

        public static void Execute(Guid WebSiteId)
        {
            lock (_locker)
            {
                var website = Kooboo.Data.GlobalDb.WebSites.Get(WebSiteId);

                if (website != null)
                {
                    Execute(website);
                }
            }
        }
        
        public static void Execute(WebSite website)
        {
            lock (_locker)
            {
                var all =  Data.GlobalDb.TaskQueue.Query.Where(o => o.WebSiteId == website.Id).SelectAll();
                Execute(website, all);
            }
        }

        public static void Execute(WebSite website, List<Queue> queueitems)
        {
            if (website == null)
            {
                foreach (var item in queueitems)
                {
                    Data.GlobalDb.TaskQueue.Delete(item.Id);
                    FailTimes.Remove(item.Id);
                }
                return; 
            }

            SiteDb siteDb = website.SiteDb();
     
            lock (_locker)
            {
                List<Guid> ItemsOk = new List<Guid>();
                foreach (var item in queueitems)
                {
                    try
                    {
                        var executor = TaskExecutor.TaskExecutorContainer.GetExecutor(item.TaskModelType);
                        if (executor != null)
                        {
                            if (executor.Execute(siteDb, item.JsonModel))
                            {
                                ItemsOk.Add(item.Id);
                            }
                            else
                            {
                                if (GetSetFailTimes(item))
                                {
                                    Data.GlobalDb.TaskQueue.Delete(item.Id);
                                    string message = item.TaskModelType + " failed 5 times";
                                    Data.GlobalDb.Notification.Add(message, message, Data.Models.NotifyType.Error, website.Id, website.OrganizationId, default(Guid));
                                    ItemsOk.Add(item.Id);
                                    Data.GlobalDb.TaskQueue.Delete(item.Id);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (GetSetFailTimes(item))
                        {
                            Data.GlobalDb.TaskQueue.Delete(item.Id);

                            Data.GlobalDb.Notification.Add(null, ex.Message, NotifyType.Error, website.Id, website.OrganizationId);

                            ItemsOk.Add(item.Id);
                        }

                    }
                }

                foreach (var item in ItemsOk)
                {
                    Data.GlobalDb.TaskQueue.Delete(item);
                    FailTimes.Remove(item);
                }
            }
        }

        private static bool GetSetFailTimes(Queue item)
        {
            int failedtimes = 0;
            if (FailTimes.ContainsKey(item.Id))
            {
                failedtimes = FailTimes[item.Id];
            }

            failedtimes += 1;

            if (failedtimes > 5)
            {
                return true;
                //Kooboo.Data.GlobalDb.TaskQueue.Delete(item.Id); 
                //FailTimes.Remove(item.Id);
            }
            else
            {
                FailTimes[item.Id] = failedtimes;
            }
            return false;
        }

        public static void Execute()
        {
            lock (_locker)
            {
                var all = Kooboo.Data.GlobalDb.TaskQueue.All();

                foreach (var item in all.GroupBy(o => o.WebSiteId))
                {
                    var websiteid = item.Key;
                    var website = Kooboo.Data.GlobalDb.WebSites.Get(websiteid);
                   Execute(website, item.ToList());
                }
            }
        }
    }
}
