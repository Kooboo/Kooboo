//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;

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

        public static void RenderFile(FrontContext context, Models.CmsFile file)
        {
            string contentType;

            contentType = !string.IsNullOrEmpty(file.ContentType) ? file.ContentType : IOHelper.MimeType(file.Name);

            context.RenderContext.Response.ContentType = contentType;

            if (file.ContentBytes != null)
            {
                context.RenderContext.Response.Body = file.ContentBytes;
            }
            else if (!string.IsNullOrEmpty(file.ContentString))
            {
                context.RenderContext.Response.Body = DataConstants.DefaultEncoding.GetBytes(file.ContentString);
            }
        }
    }
}