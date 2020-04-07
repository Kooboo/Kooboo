//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service; 
using System.Collections.Generic; 

namespace Kooboo.Sites.Render
{
  public static  class RenderPlanManager
    { 
           
        public static List<IRenderTask> GetLayoutPlan(string NameOrId, RenderContext context)
        {  
            var layout = context.WebSite.SiteDb().Layouts.GetByNameOrId(NameOrId); 
            if (layout == null)
            {
                return null; 
            }
            List<IRenderTask> renderplans = null; 

            EvaluatorOption options = RenderOptionHelper.GetLayoutOption(context, layout.Id);  

            if (options.RequireBindingInfo)
            {
                string body = DomService.ApplyKoobooId(layout.Body); 
                renderplans = RenderEvaluator.Evaluate(body, options);
            }
            else
            { 
                renderplans = Cache.RenderPlan.GetOrAddRenderPlan(context.WebSite.SiteDb(), layout.Id, () => RenderEvaluator.Evaluate(layout.Body, options)); 
            }

            return renderplans;  
        }
    }
}
