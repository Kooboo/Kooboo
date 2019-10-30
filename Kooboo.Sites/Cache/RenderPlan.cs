//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;
using Kooboo.Sites.Render;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Cache
{
    public static class RenderPlan
    {
        private static object _locker = new object();

        private static Dictionary<Guid, Dictionary<Guid, List<IRenderTask>>> _siteRenderPlans = new Dictionary<Guid, Dictionary<Guid, List<IRenderTask>>>();

        public static List<IRenderTask> GetOrAddRenderPlan(SiteDb siteDb, Guid uniqueObjectId, Func<List<IRenderTask>> evaluatePlan)
        {
            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> sitePageRenderPlan = null;
                if (_siteRenderPlans.ContainsKey(siteDb.Id))
                {
                    sitePageRenderPlan = _siteRenderPlans[siteDb.Id];
                }
                else
                {
                    sitePageRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    _siteRenderPlans[siteDb.Id] = sitePageRenderPlan;
                }

                if (!sitePageRenderPlan.ContainsKey(uniqueObjectId))
                {
                    var plan = evaluatePlan();
                    sitePageRenderPlan[uniqueObjectId] = plan;
                }
                //var plan = EvaluatePlan();
                //SitePageRenderPlan[UniqueObjectId] = plan;
                return sitePageRenderPlan[uniqueObjectId];
            }
        }

        public static List<IRenderTask> Get(SiteDb siteDb, Guid uniqueObjectId)
        {
            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> sitePageRenderPlan = null;
                if (_siteRenderPlans.ContainsKey(siteDb.Id))
                {
                    sitePageRenderPlan = _siteRenderPlans[siteDb.Id];
                }
                else
                {
                    sitePageRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    _siteRenderPlans[siteDb.Id] = sitePageRenderPlan;
                }

                return !sitePageRenderPlan.ContainsKey(uniqueObjectId) ? null : sitePageRenderPlan[uniqueObjectId];
            }
        }

        public static void Set(SiteDb siteDb, Guid uniqueObjectId, List<IRenderTask> plans)
        {
            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> sitePageRenderPlan = null;
                if (_siteRenderPlans.ContainsKey(siteDb.Id))
                {
                    sitePageRenderPlan = _siteRenderPlans[siteDb.Id];
                }
                else
                {
                    sitePageRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    _siteRenderPlans[siteDb.Id] = sitePageRenderPlan;
                }
                sitePageRenderPlan[uniqueObjectId] = plans;
            }
        }

        public static void RemovePlan(SiteDb siteDb, Guid objectId)
        {
            var indeisngid = objectId.ToString().ToHashGuid();

            lock (_locker)
            {
                Dictionary<Guid, List<IRenderTask>> siteRenderPlan = null;
                if (_siteRenderPlans.ContainsKey(siteDb.Id))
                {
                    siteRenderPlan = _siteRenderPlans[siteDb.Id];
                }
                else
                {
                    siteRenderPlan = new Dictionary<Guid, List<IRenderTask>>();
                    _siteRenderPlans[siteDb.Id] = siteRenderPlan;
                }

                siteRenderPlan.Remove(objectId);
                siteRenderPlan.Remove(indeisngid);
            }
        }

        public static void RemoveSiteDb(Guid id)
        {
            lock (_locker)
            {
                _siteRenderPlans.Remove(id);
            }
        }
    }
}