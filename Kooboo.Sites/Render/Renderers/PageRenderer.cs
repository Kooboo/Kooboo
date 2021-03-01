//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Render
{
    public class PageRenderer
    {
        public static async Task RenderAsync(FrontContext context)
        {
            // fire Page Event. .
            Page page = null;
            if (context.RenderContext.WebSite.EnableFrontEvents && context.RenderContext.IsSiteBinding)
            {
                page = FrontEvent.Manager.RaisePageEvent(FrontEvent.enumEventType.PageFinding, context.RenderContext);

                if (page == null)
                {
                    page = context.SiteDb.Pages.Get(context.Route.objectId);
                    if (page != null)
                    {
                        var changepage = FrontEvent.Manager.RaisePageEvent(FrontEvent.enumEventType.PageFound, context.RenderContext, page);

                        if (changepage != null)
                        {
                            page = changepage;
                        }
                    }
                    else
                    {
                        // page = FrontEvent.Manager.RaisePageEvent(FrontEvent.enumEventType.PageNotFound, context.RenderContext);  
                    }
                }

            }

            context.Page = page != null ? page : context.SiteDb.Pages.Get(context.Route.objectId);

            string html = null;
            if (context.Page != null)
            {
                html = await RenderEngine.RenderPageAsync(context);
            }

            if (!string.IsNullOrEmpty(html))
            {
                TextBodyRender.SetBody(context, html);
            }
            else
            {
                context.RenderContext.Response.Body = new byte[0];
            }
        }


        public static async Task<string> RenderMockAsync(RenderContext context, Page page)
        {
            FrontContext frontcontext = new FrontContext(context);
            frontcontext.Page = page;
            context.MockData = true;
            return await RenderEngine.RenderMockPageAsync(frontcontext);
        }
    }
}
