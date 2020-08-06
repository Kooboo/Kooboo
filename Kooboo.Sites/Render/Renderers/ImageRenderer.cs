//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using Kooboo.Extensions;
using Kooboo.Sites.Extensions;
using System;
using Kooboo.Data.Context;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class ImageRenderer
    {
        static ImageRenderer()
        {
            ThumbnailCache = new Dictionary<Guid, Dictionary<Guid, byte[]>>();
            _locker = new object();
        }

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
                        bytes = GetImageThumbnail(context.RenderContext, bytes, intwidth, intheight, image.Version);
                    }
                }
                else
                {
                    int intwidth = 0;

                    if (int.TryParse(width, out intwidth))
                    {
                        if (image.Height > 0 && image.Width > 0)
                        {
                            int intheight = (int)intwidth * image.Height / image.Width;

                            bytes = GetImageThumbnail(context.RenderContext, bytes, intwidth, intheight, image.Version);
                        }
                    }
                }
            }


            context.RenderContext.Response.Body = bytes;
        }

        public static byte[] GetImageThumbnail(RenderContext context, byte[] OrgBytes, int width, int height, long version)
        {
            Guid siteid = default(Guid);
            if (context != null && context.WebSite != null)
            {
                siteid = context.WebSite.Id;
            }

            Guid Hash = default(Guid);

            if (version != 0)
            {
                var bytes = BitConverter.GetBytes(version);

                byte[] all = new byte[16];

                System.Buffer.BlockCopy(bytes, 0, all, 0, 8);
                System.Buffer.BlockCopy(BitConverter.GetBytes(height), 0, all, 8, 4);
                System.Buffer.BlockCopy(BitConverter.GetBytes(width), 0, all, 12, 4);
                Hash = Lib.Security.Hash.ComputeGuid(all);
            }
            else
            {
                // no version,  file based. 
                var filehash = Lib.Security.Hash.ComputeGuid(OrgBytes);
                var unique = filehash.ToString() + height.ToString() + width.ToString();

                Hash = Lib.Security.Hash.ComputeHashGuid(unique);
            }

            var cache = GetThumbnailCache(siteid, Hash);

            if (cache != null)
            {
                return cache;
            }

            var result = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImage(OrgBytes, width, height);

            SetThumbnailCache(siteid, Hash, result);
            return result;
        }

        private static byte[] GetThumbnailCache(Guid SiteId, Guid HashId)
        {
            if (ThumbnailCache.ContainsKey(SiteId))
            {
                var list = ThumbnailCache[SiteId];
                if (list.ContainsKey(HashId))
                {
                    return list[HashId];
                }
            }
            return null;
        }

        private static object _locker;

        private static void SetThumbnailCache(Guid SiteId, Guid HashId, byte[] thumbnail)
        {
            lock (_locker)
            { 
                Dictionary<Guid, byte[]> sitecache;
                if (ThumbnailCache.ContainsKey(SiteId))
                {
                    sitecache = ThumbnailCache[SiteId];
                    sitecache[HashId] = thumbnail;
                }
                else
                {
                    sitecache = new Dictionary<Guid, byte[]>();
                    ThumbnailCache[SiteId] = sitecache;
                    sitecache[HashId] = thumbnail;
                }
            }
        }


        private static Dictionary<Guid, Dictionary<Guid, byte[]>> ThumbnailCache { get; set; }
    }

}
