//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.TaskQueue
{
    public static class QueueManager
    {
        private static object _locker = new object();

        private static Dictionary<Guid, int> _failTimes = new Dictionary<Guid, int>();

        public static void Add(object queuevalue, Guid webSiteId = default(Guid))
        {
            Kooboo.Data.Models.Queue value = new Data.Models.Queue
            {
                TaskModelType = queuevalue.GetType().FullName,
                JsonModel = Lib.Helper.JsonHelper.Serialize(queuevalue),
                WebSiteId = webSiteId
            };
            Kooboo.Data.GlobalDb.TaskQueue.AddOrUpdate(value);
        }

        public static void Execute(Guid webSiteId)
        {
            lock (_locker)
            {
                var website = Kooboo.Data.GlobalDb.WebSites.Get(webSiteId);

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
                var all = Data.GlobalDb.TaskQueue.Query.Where(o => o.WebSiteId == website.Id).SelectAll();
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
                    _failTimes.Remove(item.Id);
                }
                return;
            }

            SiteDb siteDb = website.SiteDb();

            lock (_locker)
            {
                List<Guid> itemsOk = new List<Guid>();
                foreach (var item in queueitems)
                {
                    try
                    {
                        var executor = TaskExecutor.TaskExecutorContainer.GetExecutor(item.TaskModelType);
                        if (executor != null)
                        {
                            if (executor.Execute(siteDb, item.JsonModel))
                            {
                                itemsOk.Add(item.Id);
                            }
                            else
                            {
                                if (GetSetFailTimes(item))
                                {
                                    Data.GlobalDb.TaskQueue.Delete(item.Id);
                                    string message = item.TaskModelType + " failed 5 times";
                                    Data.GlobalDb.Notification.Add(message, message, Data.Models.NotifyType.Error, website.Id, website.OrganizationId, default(Guid));
                                    itemsOk.Add(item.Id);
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

                            itemsOk.Add(item.Id);
                        }
                    }
                }

                foreach (var item in itemsOk)
                {
                    Data.GlobalDb.TaskQueue.Delete(item);
                    _failTimes.Remove(item);
                }
            }
        }

        private static bool GetSetFailTimes(Queue item)
        {
            int failedtimes = 0;
            if (_failTimes.ContainsKey(item.Id))
            {
                failedtimes = _failTimes[item.Id];
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
                _failTimes[item.Id] = failedtimes;
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