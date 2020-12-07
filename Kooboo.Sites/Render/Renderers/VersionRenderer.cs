using System; 

namespace Kooboo.Sites.Render
{
  public static  class VersionRenderer
    {
        public static void ImageVersion(FrontContext context)
        {
            if (context.RenderContext.WebSite.EnableImageBrowserCache)
            { 
                if (context.RenderContext.WebSite.ImageCacheDays > 0)
                {
                    context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddDays(context.RenderContext.WebSite.ImageCacheDays).ToString("r");
                }
                else
                {
                    // double verify...
                    var version = context.RenderContext.Request.GetValue("version");
                    if (!string.IsNullOrWhiteSpace(version))
                    {
                        context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
                    }
                }
            } 

        }

        public static void FontVersion(FrontContext context)
        {
            context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
        } 

        public static void ScriptStyleVersion(FrontContext context)
        {
            var version = context.RenderContext.Request.GetValue("version");

            if (!string.IsNullOrWhiteSpace(version))
            {
                context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
            }
        }

        public static void VideoVersion(FrontContext context)
        {
            if (context.RenderContext.WebSite.EnableVideoBrowserCache)
            { 
                context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r"); 
            } 
        }  
    }
}
