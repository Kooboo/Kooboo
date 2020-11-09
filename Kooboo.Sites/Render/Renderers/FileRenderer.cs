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
        private static long _bigsize;
        public static long BigSize
        {
            get
            {
                if (_bigsize == 0)
                {
                    _bigsize = 1024 * 1024 * 5;  // 5mb
                }
                return _bigsize;
            }
        }

        //public static void Render(FrontContext context)
        //{
        //    var file = context.SiteDb.Files.Get(context.Route.objectId);
        //    if (file == null)
        //    {
        //        return;
        //    }

        //    RenderFile(context, file);
        //}

        public static async Task RenderAsync(FrontContext context)
        {
            var file = context.SiteDb.Files.Get(context.Route.objectId, true);
            if (file == null)
            {
                return;
            }
            await RenderFile(context, file);
        }

        //file only contains column data. 
        public static async Task RenderFile(FrontContext context, Models.CmsFile file)
        {
            var sitedb = context.WebSite.SiteDb();
            string contentType = null;

            if (!string.IsNullOrEmpty(file.ContentType))
            {
                contentType = file.ContentType;
            }
            else if (!string.IsNullOrWhiteSpace(file.Name))
            {
                contentType = IOHelper.MimeType(file.Name);
            }

            if (string.IsNullOrWhiteSpace(contentType))
            {
                if (context.Route != null)
                {
                    contentType = IOHelper.MimeType(context.Route.Name);
                }
            }

            context.RenderContext.Response.ContentType = contentType;

            if (file.Size > BigSize)
            {
                var partinfo = sitedb.Files.Store.GetFieldPart(file.Id, nameof(file.ContentBytes));

                context.RenderContext.Response.FilePart = partinfo;
            }
            else
            {
                // read whole content. 
                var FileContent = await sitedb.FilePool.GetAsync(file.Id);

                if (FileContent.ContentBytes != null)
                {
                    context.RenderContext.Response.Body = FileContent.ContentBytes;
                }
                else if (!string.IsNullOrEmpty(FileContent.ContentString))
                {
                    context.RenderContext.Response.Body = DataConstants.DefaultEncoding.GetBytes(FileContent.ContentString);
                }
            }



            // cache for font.
            if (contentType != null)
            {
                if (contentType.ToLower().Contains("font"))
                {
                    context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
                }
                else if (contentType.ToLower().Contains("image"))
                {
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
            }

        }
    }
}
