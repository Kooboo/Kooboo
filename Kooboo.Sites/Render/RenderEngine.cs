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

            var option = RenderOptionHelper.GetPageOption(context);

            if (option.RequireBindingInfo)
            {
                string html = DomService.ApplyKoobooId(context.Page.Body);
                RenderPlan = RenderEvaluator.Evaluate(html, option);
                var traceability = new ComponentTrace(context.Page.Id.ToString(), "page");
                var bindingTask = new BindingRenderTask(traceability, new Dictionary<string, string> { { "scope", "true" } });
                RenderPlan.Insert(0, bindingTask);
                RenderPlan.Add(bindingTask.BindingEndRenderTask);

                result = RenderHelper.Render(RenderPlan, context.RenderContext);
                result = DomService.EnsureDocType(result);
            }
            else
            {
                RenderPlan = Cache.RenderPlan.GetOrAddRenderPlan(context.SiteDb, context.Page.Id, () => RenderEvaluator.Evaluate(context.Page.Body, option));

                // check the cache. 
                if (context.Page.EnableCache)
                {
                    long version = context.Page.Version;

                    var minutes = context.Page.CacheMinutes;

                    if (!context.Page.CacheByVersion)
                    {
                        version = -1;
                    }

                    result = Kooboo.Sites.Render.PageCache.PageCache.Get(context.SiteDb.Id, context.Page.Id, version, minutes);

                    if (string.IsNullOrEmpty(result))
                    {
                        result = RenderHelper.Render(RenderPlan, context.RenderContext);
                        Kooboo.Sites.Render.PageCache.PageCache.Set(context.SiteDb.Id, context.Page.Id, result, context.Page.Version);
                    }
                }
                else
                {
                    result = RenderHelper.Render(RenderPlan, context.RenderContext);
                }

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

        public static async Task<string> RenderMockPageAsync(FrontContext context)
        {
            if (context.Page.Parameters.Count > 0)
            {
                context.RenderContext.DataContext.Push(context.Page.Parameters);
            }

            string result = string.Empty;

            List<IRenderTask> RenderPlan = null;

            var option = RenderOptionHelper.GetPageOption(context);


            RenderPlan = Cache.RenderPlan.GetOrAddRenderPlan(context.SiteDb, context.Page.Id, () => RenderEvaluator.Evaluate(context.Page.Body, option));

            // check the cache. 
            if (context.Page.EnableCache)
            {
                long version = context.Page.Version;

                var minutes = context.Page.CacheMinutes;

                if (!context.Page.CacheByVersion)
                {
                    version = -1;
                }

                result = Kooboo.Sites.Render.PageCache.PageCache.Get(context.SiteDb.Id, context.Page.Id, version, minutes);

                if (string.IsNullOrEmpty(result))
                {
                    result = RenderHelper.Render(RenderPlan, context.RenderContext);
                    Kooboo.Sites.Render.PageCache.PageCache.Set(context.SiteDb.Id, context.Page.Id, result, context.Page.Version);
                }
            }
            else
            {
                result = RenderHelper.Render(RenderPlan, context.RenderContext);
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
    }
}
