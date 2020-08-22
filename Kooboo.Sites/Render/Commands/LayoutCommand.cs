//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Render.Commands
{
    public class LayoutCommand : ICommand
    {
        public string Name
        {
            get
            {
                return "layout";
            }
        }

        public string Execute(RenderContext context, Dictionary<string, string> Paras, EvaluatorOption options)
        {
            if (Paras != null && Paras.Count() > 0)
            {
                Dictionary<string, string> datavalue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in Paras)
                {
                    if (item.Key.ToLower() != "name" && item.Key.ToLower() != "id")
                    {
                        datavalue.Add(item.Key, item.Value);
                    }
                }
                if (datavalue.Count() > 0)
                {
                    context.DataContext.Push(datavalue);
                }
            }

            string layoutNameOrId = GetLayoutNameOrId(Paras);
            string layoutbody = null;

            if (!string.IsNullOrEmpty(layoutNameOrId))
            {
                layoutbody = GetSourceFromDb(context, layoutNameOrId);

                if (!string.IsNullOrEmpty(layoutbody))
                {
                    List<IRenderTask> renderplan;

                    if (options == null)
                    {
                        options = new EvaluatorOption();
                        options.RenderUrl = false;
                        options.RenderHeader = false;
                        options.EnableImageBrowserCache = true;
                        options.EnableJsCssBrowserCache = true;
                    }
                     

                    Guid sourceid = Lib.Security.Hash.ComputeHashGuid(layoutbody);  // GetLayoutGuid(layoutNameOrId); 

                    if (context.Request.Channel == RequestChannel.InlineDesign)
                    {
                        layoutbody = DomService.ApplyKoobooId(layoutbody);
                        options.RequireBindingInfo = true;
                        renderplan = RenderEvaluator.Evaluate(layoutbody, options);
                    }
                    else
                    {
                        SiteDb sitedb;
                        if (context.WebSite == null)
                        {
                            var site = new WebSite() { Name = "__Koobootemp" };
                            sitedb = site.SiteDb();
                        }
                        else
                        {
                            sitedb = context.WebSite.SiteDb();
                        }

                        renderplan = Cache.RenderPlan.GetOrAddRenderPlan(sitedb, sourceid, () => RenderEvaluator.Evaluate(layoutbody, options));
                    }
                    return RenderHelper.Render(renderplan, context);
                }
            }

            return null;
        }

        private Guid GetLayoutGuid(string LayoutNameOrId)
        {
            // Guid LayoutId = Data.IDGenerator.Generate(this.Name, this.ConstType);
            Guid LayoutId;
            if (System.Guid.TryParse(LayoutNameOrId, out LayoutId))
            {
                return LayoutId;
            }
            else
            {
                return Kooboo.Data.IDGenerator.Generate(LayoutNameOrId, ConstObjectType.Layout);
            }
        }

        private static string GetLayoutNameOrId(Dictionary<string, string> Paras)
        {
            string layoutid = null;
            foreach (var item in Paras)
            {
                if (item.Key.ToLower() == "name" || item.Key.ToLower() == "id")
                {
                    layoutid = item.Value;
                }
            }

            if (string.IsNullOrEmpty(layoutid))
            {
                foreach (var item in Paras)
                {
                    if (item.Key.ToLower().StartsWith("name") || item.Key.ToLower().StartsWith("id"))
                    {
                        layoutid = item.Value;
                    }
                }
            }

            return layoutid;
        }

        public static string GetSourceFromDb(RenderContext context, string layoutNameOrId)
        {
            var sourceprovider = context.GetItem<ICommandSourceProvider>("commandsource");

            if (sourceprovider == null)
            {
                sourceprovider = new DBCommandSourceProvider();
            }

            return sourceprovider.GetLayout(context, layoutNameOrId);
        }

        private static string GetPrefixLayoutSource(RenderContext context, string filename)
        {
            string lower = filename.ToLower();
            // check prefix
            if (lower.StartsWith("/_layout/"))
            {
                lower = lower.Substring("/_layout/".Length);
            }
            else if (lower.StartsWith("/layout/"))
            {
                lower = lower.Substring("/layout/".Length);
            }
            else if (lower.StartsWith("_layout/"))
            {
                lower = lower.Substring("_layout/".Length);
            }
            else if (lower.StartsWith("layout/"))
            {
                lower = lower.Substring("layout/".Length);
            }
            var layout = context.WebSite.SiteDb().Layouts.GetByNameOrId(lower);
            return layout != null ? layout.Body : null;
        }

    }
}
