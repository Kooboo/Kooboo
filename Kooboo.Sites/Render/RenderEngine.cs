//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using System.Collections.Generic;
using Kooboo.Sites.Service;
using System;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using Kooboo.Data.Context;
using System.Linq;

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
                    Dictionary<string, string> querystring = null;

                    if (!string.IsNullOrWhiteSpace(context.Page.CacheQueryKeys))
                    {
                        //querystring = RequestManager.GetQueryString(context.RenderContext.Request);
                        querystring = GetParaValues(context.RenderContext, context.Page.CacheKeys);
                    }

                    if (context.Page.CacheByVersion)
                    {
                        result = PageCache.PageCache.GetByVersion(context.SiteDb.Id, context.Page.Id, context.Page.Version, querystring);
                    }
                    else
                    {
                        result = PageCache.PageCache.GetByMinutes(context.SiteDb.Id, context.Page.Id, context.Page.CacheMinutes, querystring, context.Page.Version);
                    }

                    if (string.IsNullOrEmpty(result))
                    {
                        result = RenderHelper.Render(RenderPlan, context.RenderContext);

                        Kooboo.Sites.Render.PageCache.PageCache.Set(context.SiteDb.Id, context.Page.Id, result, context.Page.Version, querystring);
                    }


                    // cache result may have replacement. 
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
            option.Evaluators = EvaluatorContainer.MockData;

            context.RenderContext.MockData = true;

            RenderPlan = RenderEvaluator.Evaluate(context.Page.Body, option);

            result = RenderHelper.Render(RenderPlan, context.RenderContext);

            return result;
        }

        public static Dictionary<string, string> GetParaValues(RenderContext context, string[] keys)
        {
            if (keys == null)
            {
                return null;
            }
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var item in keys)
            {
                var value = RequestManager.GetValue(context.Request, item);
                result[item] = value;
            } 
            return result;
        }
    }
}
