//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Sites.Render;
using System.Collections.Concurrent;

namespace Kooboo.Render
{

    public static class RenderPlanCache
    {
        private static object _locker = new object();

        private static ConcurrentDictionary<Guid, RenderPlanWrapper> RenderPlans = new ConcurrentDictionary<Guid, RenderPlanWrapper>();

        public static List<IRenderTask> GetOrAddRenderPlan(Guid UniqueObjectId, Func<List<IRenderTask>> EvaluatePlan)
        {

            lock (_locker)
            {
                if (RenderPlans.TryGetValue(UniqueObjectId, out var PlanWrapper))
                {
                    if (PlanWrapper != null && PlanWrapper.LastModified > DateTime.Now.AddDays(-3))
                    {
                        TryCleanCache();  
                        return PlanWrapper.Tasks;
                    }
                }

                var plan = EvaluatePlan();
                var wrapper = new RenderPlanWrapper(plan);
                RenderPlans.TryAdd(UniqueObjectId, wrapper);

                TryCleanCache(); 
                return plan;
            }
        }

        private static int beingUsed = 0;

        private static DateTime lastCleanTime = DateTime.Now;

        private static void TryCleanCache()
        {

            if (lastCleanTime > DateTime.Now.AddDays(-3))
            {
                return;
            }

            if (0 == System.Threading.Interlocked.Exchange(ref beingUsed, 1))
            {
                if (lastCleanTime > DateTime.Now.AddDays(-3))
                {
                    return;
                }

                lastCleanTime = DateTime.Now;

                System.Threading.Tasks.Task.Run(() => _cleanCache()); 

                System.Threading.Interlocked.Exchange(ref beingUsed, 0);
            }
        }

        private static void _cleanCache()
        {  
            HashSet<Guid> RemoveItems = new HashSet<Guid>();
            DateTime TooOldMarkTime = DateTime.Now.AddDays(-3);

            foreach (var item in RenderPlans)
            {
                if (item.Value == null || item.Value.LastModified < TooOldMarkTime)
                {
                    RemoveItems.Add(item.Key);
                }
            }

            foreach (var item in RemoveItems)
            {
                RenderPlans.TryRemove(item, out var items);
            }
        }


    }

    public class RenderPlanWrapper
    {
        public RenderPlanWrapper(List<IRenderTask> tasks)
        {
            this.Tasks = tasks;
            this.LastModified = DateTime.Now;
        }

        public List<IRenderTask> Tasks;

        public DateTime LastModified { get; set; }
    }

}
