//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using Kooboo.Extensions;
using Kooboo.Sites.Extensions;
using System;

namespace Kooboo.Sites.Render
{
    public class ImageRenderer
    {
        public async static Task RenderAsync(FrontContext context)
        {
            var image = await context.SiteDb.ImagePool.GetAsync(context.Route.objectId);

            if (image == null || image.ContentBytes == null)
            {
                image = await context.SiteDb.Images.GetAsync(context.Route.objectId);
            }
            //var image = context.SiteDb.Images.Get(context.Route.objectId); 
            if (context.RenderContext.WebSite.EnableImageLog)
            {
                if (context.RenderContext.Request.Channel == Data.Context.RequestChannel.Default)
                {
                    Kooboo.Data.Models.ImageLog log = new Data.Models.ImageLog();
                    log.ClientIP = context.RenderContext.Request.IP;
                    log.Url = context.RenderContext.Request.RawRelativeUrl;
                    log.StartTime = DateTime.Now;

                    if (image != null)
                    {
                        log.Size = image.Size;
                        log.ImageId = image.Id;
                    }
                    context.RenderContext.WebSite.SiteDb().ImageLog.Add(log);
                }
            }

            if (image == null)
            {
                return;
            }

            RenderImage(context, image);

            context.RenderContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.RenderContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");

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

        public static void Render(FrontContext context)
        {
            var image = context.SiteDb.ImagePool.Get(context.Route.objectId);

            if (image == null || image.ContentBytes == null)
            {
                image = context.SiteDb.Images.Get(context.Route.objectId);
            }
            if (context.RenderContext.WebSite.EnableImageLog)
            {
                if (context.RenderContext.Request.Channel == Data.Context.RequestChannel.Default)
                {
                    Kooboo.Data.Models.ImageLog log = new Data.Models.ImageLog();
                    log.ClientIP = context.RenderContext.Request.IP;
                    log.Url = context.RenderContext.Request.RawRelativeUrl;
                    log.StartTime = DateTime.Now;

                    if (image != null)
                    {
                        log.Size = image.Size;
                        log.ImageId = image.Id;
                    }
                    context.RenderContext.WebSite.SiteDb().ImageLog.Add(log);
                }
            }

            if (image == null)
            {
                return;
            }
            RenderImage(context, image);
        }

        public static void RenderImage(FrontContext context, Models.Image image)
        {
            context.RenderContext.Response.ContentType = "image";

            if (Kooboo.Lib.Helper.UrlHelper.GetFileType(image.Extension) == Lib.Helper.UrlHelper.UrlFileType.Image)
            {
                string imagetype = image.Extension;
                if (imagetype.StartsWith("."))
                {
                    imagetype = imagetype.Substring(1);
                }
                context.RenderContext.Response.ContentType = context.RenderContext.Response.ContentType + "/" + imagetype;
            }

            //"image/svg+xml"
            if (image.Extension == "svg" || image.Extension == ".svg")
            {
                context.RenderContext.Response.ContentType += "+xml";
            }

            var bytes = image.ContentBytes;
             
            var width = context.RenderContext.Request.Get("width"); 
            if (!string.IsNullOrEmpty(width))
            {
                var height = context.RenderContext.Request.Get("height"); 
                 
                 if (!string.IsNullOrWhiteSpace(height))
                {
                    int intwidth = 0;
                    int intheight = 0; 
                    if (int.TryParse(width, out intwidth) && int.TryParse(height, out intheight))
                    {
                     bytes = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImage(bytes, intwidth, intheight);
                    }
                }
                 else
                {
                    int intwidth = 0;
         
                    if (int.TryParse(width, out intwidth))
                    {
                        if (image.Height>0 && image.Width >0)
                        {  
                            int intheight =   (int)intwidth * image.Height / image.Width;  
                            bytes = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImage(bytes, intwidth, intheight);
                        } 
                    }
                }
            }
             

            context.RenderContext.Response.Body = bytes;
        }



    }
}
