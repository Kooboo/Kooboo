//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 
using Kooboo.Sites.Render.Renderers; 

namespace Kooboo.Sites.Render
{
    public static class StyleRenderer
    {
        public static void Render(FrontContext context)
        {
            var css = context.SiteDb.Styles.Get(context.Route.objectId);
            context.RenderContext.Response.ContentType = "text/css;charset=utf-8";

            if (css != null && css.Body != null)
            {
                var body = GetBody(css);

                if (context.RenderContext.WebSite != null && context.RenderContext.WebSite.EnableJsCssCompress)
                {
                    if (css != null)
                    {
                        body = CompressCache.Get(css.Id, css.Version, body, CompressType.css);
                    }
                }
                TextBodyRender.SetBody(context, body); 
                VersionRenderer.ScriptStyleVersion(context); 
            }
        } 

        public static string GetBody(Kooboo.Sites.Models.Style style)
        {
            if (style == null || string.IsNullOrEmpty(style.Body))
            {
                return null;
            }

            if (style.Extension == null || style.Extension == "css" || style.Extension == ".css")
            {
                return style.Body;
            }
            else
            {
                var styleEngines = Kooboo.Sites.Engine.Manager.GetStyle();

                var find = styleEngines.Find(o => o.Extension == style.Extension);
                if (find != null)
                {
                    return find.Execute(null, style.Body);
                }
                else
                {
                    return style.Body;
                }
            }

        }

    }
}
