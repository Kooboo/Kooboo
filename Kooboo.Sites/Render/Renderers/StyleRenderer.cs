//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;

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
                var bytes = Encoding.UTF8.GetBytes(css.Body);

                //if (context.RenderContext.EnableTextGZip)
                //{
                //    context.RenderContext.Response.Headers.Add("Content-encoding", "GZip");
                //    context.RenderContext.Response.OrginalLength = bytes.Length; 
                //    var stream = new System.IO.MemoryStream(bytes);

                //context.RenderContext.Response.Stream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionLevel.Fastest);

                //}
                //else
                //{
                    // var body = GetBody(css);
                    context.RenderContext.Response.Body = bytes;
               // }
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
