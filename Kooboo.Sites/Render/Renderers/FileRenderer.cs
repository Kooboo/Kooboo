//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using Kooboo.Extensions;
using Kooboo.Sites.Extensions;
using Kooboo.Lib;
using Kooboo.Lib.Helper;
using System;

namespace Kooboo.Sites.Render
{
    public class FileRenderer
    {
        public static void Render(FrontContext context)
        {
            var file = context.SiteDb.Files.Get(context.Route.objectId);
            if (file == null)
            {
                return;
            }

            RenderFile(context, file);
        }

        public static async Task RenderAsync(FrontContext context)
        {
            var file = await context.SiteDb.Files.GetAsync(context.Route.objectId);
            if (file == null)
            {
                return;
            } 
            RenderFile(context, file);
        }

        public static void RenderFile(FrontContext context, Models.CmsFile file)
        {
            string contentType;

            if (!string.IsNullOrEmpty(file.ContentType))
            {
                contentType = file.ContentType;
            }
            else
            {
                contentType = IOHelper.MimeType(file.Name);
            }

            context.RenderContext.Response.ContentType = contentType;

            if (file.ContentBytes != null)
            {
                context.RenderContext.Response.Body = file.ContentBytes;
            }
            else if (!string.IsNullOrEmpty(file.ContentString))
            {
                context.RenderContext.Response.Body = DataConstants.DefaultEncoding.GetBytes(file.ContentString);
            }

            // cache for font.
            if (contentType !=null && contentType.ToLower().Contains("font"))
            {
                context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
            }

        }
    }
}
