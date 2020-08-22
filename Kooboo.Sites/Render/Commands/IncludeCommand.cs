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
    public class IncludeCommand : ICommand
    { 
        public string Name
        {
            get
            {
                return "include"; 
            }
        }
          
        public string Execute(RenderContext context, Dictionary<string, string> Paras, EvaluatorOption options)
        { 
            if (Paras != null && Paras.Count() > 0)
            {
                Dictionary<string, string> datavalue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in Paras)
                {
                    if (item.Key.ToLower() != "file" && item.Key.ToLower() != "virtual")
                    {
                        datavalue.Add(item.Key, item.Value); 
                    }
                }
                if (datavalue.Count() > 0)
                {
                   context.DataContext.Push(datavalue);
                }
            }

            var file = GetFilePath(context, Paras);

            var source = GetSourceFromDb(context, file); 

            if (!string.IsNullOrEmpty(source))
            {
                List<IRenderTask> renderplan; 

                if (options == null)
                {
                    options = new EvaluatorOption();
                    options.RenderUrl = true;
                    options.RenderHeader = false;
                }

                
                Guid sourceid = Lib.Security.Hash.ComputeGuidIgnoreCase(source); 
            
                if (context.Request.Channel == RequestChannel.InlineDesign)
                {
                    source = DomService.ApplyKoobooId(source);
                    options.RequireBindingInfo = true;
                    renderplan = RenderEvaluator.Evaluate(source, options); 
                    
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
                        sitedb  = context.WebSite.SiteDb();
                    }
                    
                    renderplan = Cache.RenderPlan.GetOrAddRenderPlan(sitedb, sourceid, () => RenderEvaluator.Evaluate(source, options));
                }

               return RenderHelper.Render(renderplan, context);

            }

            return null;
        }

        public string GetFilePath(RenderContext context, Dictionary<string, string> Paras)
        {
            if (Paras.ContainsKey("file"))
            {
                string file = Paras["file"];
                string relativefile = Kooboo.Lib.Helper.UrlHelper.Combine(context.Request.RelativeUrl, file);

                return relativefile;  
            }
            else if (Paras.ContainsKey("virtual"))
            {
                string file = Paras["virtual"];
                return file; 
            }
            else
            {
                foreach (var item in Paras)
                {
                    if (item.Key.ToLower() =="name"|| item.Key.ToLower()== "id")
                    {
                        return item.Value; 
                    }
                }
            }

            return null; 
        }
         
        public static string GetSourceFromDb(RenderContext context, string filename)
        {
            /// try get view... 
            /// 
          var sourceprovider =   context.GetItem<ICommandSourceProvider>("commandsource");

            if (sourceprovider == null)
            {
                sourceprovider = new DBCommandSourceProvider();  
            }

            return sourceprovider.GetString(context, filename); 
        }
          
    }
}
