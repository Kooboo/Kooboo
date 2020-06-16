using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
    public class FileVersion
    {
        public RenderContext context { get; set; }

        public FileVersion(RenderContext Context)
        {
            this.context = Context;
        }

        public string ImageVersionSrc(string imgSrc)
        {
            if (!this.context.WebSite.EnableImageBrowserCache || this.context.WebSite.ImageCacheDays > 0)
            {
                return imgSrc;
            }

            if (imgSrc == null)
            {
                return null;
            }

            var lower = imgSrc.ToLower();
            if (lower.StartsWith("http://") || lower.StartsWith("https://"))
            {
                imgSrc = Lib.Helper.UrlHelper.RelativePath(imgSrc);
            }

            var sitedb = context.WebSite.SiteDb();


            var obj = sitedb.Images.GetByUrl(imgSrc);

            if (obj != null)
            {
                if (imgSrc.Contains("?"))
                {
                    return imgSrc + "&version=" + obj.Version.ToString();
                }
                else
                {
                    return imgSrc + "?version=" + obj.Version.ToString();
                }
            }
            else
            {
                var kfilepath = Kooboo.Sites.Systems.SystemRender.GetFileFullPath(context, imgSrc);

                if (!string.IsNullOrWhiteSpace(kfilepath) && System.IO.File.Exists(kfilepath))
                {
                    var fileinfo = new System.IO.FileInfo(kfilepath);
                    if (fileinfo != null)
                    {
                        var ver = fileinfo.LastWriteTime.Ticks.ToString();
                        if (imgSrc.Contains("?"))
                        {
                            return imgSrc + "&version=" + ver;
                        }
                        else
                        {
                            return imgSrc + "?version=" + ver;
                        }
                    }
                }

            }

            return imgSrc;
        }

        //public long GetImageVersion(string Id)
        //{
        //    var sitedb = context.WebSite.SiteDb();
        //    var img = sitedb.Images.GetByNameOrId(Id);
        //    if (img != null)
        //    {
        //        return img.Version;
        //    }
        //    return -1;
        //}


    }
}
