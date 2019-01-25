//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Render; 

namespace Kooboo.Sites.Cache
{
    public static class RenderPlan
    {
        private static object _locker = new object();

        private static Dictionary<Guid, Dictionary<Guid, List<IRenderTask>>> SiteRenderPlans = new Dictionary<Guid, Dictionary<Guid, List<IRenderTask>>>();

        public static List<IRenderTask> GetOrAddRenderPlan(SiteDb SiteDb, Guid UniqueObjectId, Func<List<IRenderTask>> EvaluatePlan)
        {
            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> SitePageRenderPlan = null;
                if (SiteRenderPlans.ContainsKey(SiteDb.Id))
                {
                    SitePageRenderPlan = SiteRenderPlans[SiteDb.Id];
                }
                else
                {
                    SitePageRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    SiteRenderPlans[SiteDb.Id] = SitePageRenderPlan;
                }

                if (!SitePageRenderPlan.ContainsKey(UniqueObjectId))
                {
                    var plan = EvaluatePlan();
                    SitePageRenderPlan[UniqueObjectId] = plan;
                }
                //var plan = EvaluatePlan();
                //SitePageRenderPlan[UniqueObjectId] = plan;
                return SitePageRenderPlan[UniqueObjectId];
            }
        }

        public static List<IRenderTask> Get(SiteDb SiteDb, Guid UniqueObjectId)
        {
            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> SitePageRenderPlan = null;
                if (SiteRenderPlans.ContainsKey(SiteDb.Id))
                {
                    SitePageRenderPlan = SiteRenderPlans[SiteDb.Id];
                }
                else
                {
                    SitePageRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    SiteRenderPlans[SiteDb.Id] = SitePageRenderPlan;
                }

                if (!SitePageRenderPlan.ContainsKey(UniqueObjectId))
                {
                    return null;
                }
                else
                {
                    return SitePageRenderPlan[UniqueObjectId];
                }
            }
        }

        public static void Set(SiteDb SiteDb, Guid UniqueObjectId, List<IRenderTask> plans)
        {
            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> SitePageRenderPlan = null;
                if (SiteRenderPlans.ContainsKey(SiteDb.Id))
                {
                    SitePageRenderPlan = SiteRenderPlans[SiteDb.Id];
                }
                else
                {
                    SitePageRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    SiteRenderPlans[SiteDb.Id] = SitePageRenderPlan;
                }  
               SitePageRenderPlan[UniqueObjectId] = plans; 
            }
        }
        
        public static void RemovePlan(SiteDb SiteDb, Guid ObjectId)
        {
            var indeisngid = ObjectId.ToString().ToHashGuid();

            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> SiteRenderPlan = null;
                if (SiteRenderPlans.ContainsKey(SiteDb.Id))
                {
                    SiteRenderPlan = SiteRenderPlans[SiteDb.Id];

                }
                else
                {
                    SiteRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    SiteRenderPlans[SiteDb.Id] = SiteRenderPlan;
                }

                SiteRenderPlan.Remove(ObjectId);
                SiteRenderPlan.Remove(indeisngid);
            }
        }

        public static void RemoveSiteDb(Guid id)
        {
            lock (_locker)
            {
                SiteRenderPlans.Remove(id);
            }
        }
    }
}
