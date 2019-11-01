//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Render;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.Spa
{
    public static class RenderPlanCache
    {
        private static object _locker = new object();

        private static Dictionary<Guid, List<IRenderTask>> _renderPlans = new Dictionary<Guid, List<IRenderTask>>();

        public static List<IRenderTask> GetOrAddRenderPlan(Guid uniqueObjectId, Func<List<IRenderTask>> evaluatePlan)
        {
            lock (_locker)
            {
                if (!_renderPlans.ContainsKey(uniqueObjectId))
                {
                    var plan = evaluatePlan();
                    _renderPlans[uniqueObjectId] = plan;
                }
                return _renderPlans[uniqueObjectId];
            }
        }
    }
}