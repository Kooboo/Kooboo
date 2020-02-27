//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using System.Collections.Generic;
using Kooboo.Sites.Service;
using System;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;

namespace Kooboo.Sites.Render
{
    public static class RenderEngine
    {

        public static async Task<string> RenderPageAsync(FrontContext context)
        {
            if (context.Page.Parameters.Count > 0)
            {
                context.RenderContext.DataContext.Push(context.Page.Parameters);
            }

            string result = string.Empty;

            List<IRenderTask> RenderPlan = null;

            if (context.RenderContext.Request.Channel != Data.Context.RequestChannel.InlineDesign)
            {
                RenderPlan = Cache.RenderPlan.GetOrAddRenderPlan(context.SiteDb, context.Page.Id, () => RenderEvaluator.Evaluate(context.Page.Body, GetPageOption(context)));

                result = RenderHelper.Render(RenderPlan, context.RenderContext);
            }
            else
            {
                string html = DomService.ApplyKoobooId(context.Page.Body);
                RenderPlan = RenderEvaluator.Evaluate(html, GetPageOption(context));
                var traceability = new ComponentTrace(context.Page.Id.ToString(), "page");
                var bindingTask = new BindingRenderTask(traceability, new Dictionary<string, string> { { "scope", "true" } });
                RenderPlan.Insert(0, bindingTask);
                RenderPlan.Add(bindingTask.BindingEndRenderTask);
                result = RenderHelper.Render(RenderPlan, context.RenderContext);
                result = DomService.EnsureDocType(result);
            }


            if (context.Page.Type == Models.PageType.RichText)
            {
                //special for richtext editor. meta name = "viewport" content = "width=device-width, initial-scale=1"
                var header = new Models.HtmlHeader();
                Dictionary<string, string> content = new Dictionary<string, string>();
                content.Add("", "width=device-width, initial-scale=1");
                header.Metas.Add(new Models.HtmlMeta() { name = "viewport", content = content });

                result = HtmlHeadService.SetHeaderToHtml(result, header);
            }

            return result;
        }

        private static EvaluatorOption GetPageOption(FrontContext context)
        {
            EvaluatorOption renderoption = new EvaluatorOption();

            if (context.WebSite != null && context.WebSite.EnableSitePath)
            {
                renderoption.RenderUrl = true;
            }
            else
            {
                renderoption.RenderUrl = false;
            }

            if (context.Page.Headers.HasValue())
            {
                if (context.Page.HasLayout)
                {
                    renderoption.RenderHeader = false;
                }
                else
                {
                    renderoption.RenderHeader = true;
                }
            }
            else
            {
                renderoption.RenderHeader = false;
            }

            //renderoption.RenderHeader = context.Page.Headers.HasValue();     

            renderoption.RequireBindingInfo = context.RenderContext.Request.Channel == Data.Context.RequestChannel.InlineDesign;
            renderoption.OwnerObjectId = context.Page.Id;
            return renderoption;
        }
    }
}
