//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Render.Renderers;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Systems
{
    public class SystemRender
    {
        public static async Task Render(FrontContext context)
        {
            //systemroute.Name = "__kb/{objecttype}/{nameorid}"; defined in Routes. 
            var paras = context.Route.Parameters;
            var strObjectType = paras.GetValue("objecttype");
            byte constType = ConstObjectType.Unknown;
            if (!byte.TryParse(strObjectType, out constType))
            {
                constType = ConstTypeContainer.GetConstType(strObjectType);
            }
            var id = paras.GetValue("nameorid");

            switch (constType)
            {
                case ConstObjectType.ResourceGroup:
                    ResourceGroupRender(context, id);
                    return;
                case ConstObjectType.View:
                    await ViewRender(context, id, paras);
                    return;
                case ConstObjectType.Image:
                    {
                        ImageRender(context, id, paras);
                        return;
                    }
                case ConstObjectType.CmsFile:
                    {
                        FileRender(context, id, paras);
                        return;
                    }
                case ConstObjectType.kfile:
                    {
                        KFileRender(context, paras);
                        return;
                    }

                //case ConstObjectType.TextContent:
                //    {
                //        TextContentRender(context, id, paras);
                //        return;
                //    }
                default:
                    DefaultRender(context, constType, strObjectType, id, paras);
                    break;
            }
        }

        private static string GetObjectType(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("objecttype"))
            {
                return parameters["objecttype"];
            }
            else if (parameters.ContainsKey("type"))
            {
                return parameters["type"];
            }
            return null;
        }

        public static void ResourceGroupRender(FrontContext context, string id)
        {
            var db = context.SiteDb;
            var group = db.ResourceGroups.GetByNameOrId(id, ConstObjectType.Style);
            if (group == null)
            {
                group = db.ResourceGroups.GetByNameOrId(id, ConstObjectType.Script);
            }
            if (group == null)
            {
                group = db.ResourceGroups.TableScan.Where(o => Lib.Helper.StringHelper.IsSameValue(o.Name, id)).FirstOrDefault();
            }
            if (group == null)
            {
                return;
            }
            IRepository repo = null;
            string spliter = "\r\n";
            switch (group.Type)
            {
                case ConstObjectType.Style:
                    context.RenderContext.Response.ContentType = "text/css;charset=utf-8";
                    repo = context.SiteDb.Styles as IRepository;
                    break;
                case ConstObjectType.Script:
                    context.RenderContext.Response.ContentType = "text/javascript;charset=utf-8";
                    repo = context.SiteDb.Scripts as IRepository;
                    spliter = ";\r\n";//need split with newline ,otherwise two different combinations of comments will report an error
                    break;
                default:
                    break;
            }

            StringBuilder sb = new StringBuilder();

            long totalversion = 0;

            foreach (var item in group.Children.OrderBy(o => o.Value))
            {
                var route = context.SiteDb.Routes.Get(item.Key);
                if (route != null)
                {
                    var siteobject = repo.Get(route.objectId);
                    if (siteobject != null)
                    {
                        if (siteobject is ITextObject)
                        {
                            var text = siteobject as ITextObject;
                            sb.Append(text.Body);
                            sb.Append(spliter);
                        }

                        if (siteobject is ICoreObject)
                        {
                            var core = siteobject as ICoreObject;
                            totalversion += core.Version;
                        }
                    }

                }
            }

            string result = sb.ToString();

            if (context.RenderContext.WebSite != null && context.RenderContext.WebSite.EnableJsCssCompress)
            {

                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (group.Type == ConstObjectType.Style)
                    {
                        result = CompressCache.Get(group.Id, totalversion, result, CompressType.css);
                    }
                    else if (group.Type == ConstObjectType.Script)
                    {
                        result = CompressCache.Get(group.Id, totalversion, result, CompressType.js);
                    }
                }
            }


            TextBodyRender.SetBody(context, result);

            var version = context.RenderContext.Request.GetValue("version");

            if (!string.IsNullOrWhiteSpace(version))
            {
                context.RenderContext.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
            }

        }

        public static async Task ViewRender(FrontContext context, string NameOrId, Dictionary<string, string> Parameters)
        {
            var action = Parameters.GetValue("action");

            if (string.IsNullOrEmpty(action) || action == "render")
            {
                System.Guid viewid = default(Guid);
                System.Guid.TryParse(NameOrId, out viewid);
                if (viewid == default(Guid))
                {
                    viewid = Kooboo.Data.IDGenerator.Generate(NameOrId, ConstObjectType.View);
                }
                context.Route.DestinationConstType = ConstObjectType.View;
                context.Route.objectId = viewid;
                await ViewRenderer.Render(context);
            }
            else
            {
                var view = context.SiteDb.Views.GetByNameOrId(NameOrId);
                if (view != null)
                {
                    context.RenderContext.Response.ContentType = "text/html;charset=utf-8";
                    context.RenderContext.Response.Body = DataConstants.DefaultEncoding.GetBytes(view.Body);
                }
            }
        }

        public static void ImageRender(FrontContext context, string NameOrId, Dictionary<string, string> Parameters)
        {
            Guid ImageId;
            Image image = null;

            if (System.Guid.TryParse(NameOrId, out ImageId))
            {
                image = context.SiteDb.Images.Get(ImageId) as Image;
            }

            long logid = -1;

            if (long.TryParse(NameOrId, out logid))
            {
                var logentry = context.SiteDb.Log.Get(logid);
                var repo = context.SiteDb.Images as IRepository;
                image = repo.GetByLog(logentry) as Image;
            }

            if (image != null)
            {
                Kooboo.Sites.Render.ImageRenderer.RenderImage(context, image);
            }
        }

        public static void FileRender(FrontContext context, string NameOrId, Dictionary<string, string> Parameters)
        {
            Guid FileId;
            CmsFile file = null;

            if (System.Guid.TryParse(NameOrId, out FileId))
            {
                file = context.SiteDb.Files.Get(FileId, true) as CmsFile;
            }

            long logid = -1;

            if (long.TryParse(NameOrId, out logid))
            {
                var logentry = context.SiteDb.Log.Get(logid);
                var repo = context.SiteDb.Images as IRepository;
                file = repo.GetByLog(logentry) as CmsFile;
            }

            if (file != null)
            {
                FileRenderer.RenderFile(context, file);
            }
        }

        public static void TextContentRender(FrontContext context, string NameOrId, Dictionary<string, string> Parameters)
        {
            throw new NotImplementedException();
        }

        public static void DefaultRender(FrontContext context, byte ConstType, string objectType, string NameOrId, Dictionary<string, string> Parameters)
        {

            var modeltype = Service.ConstTypeService.GetModelType(ConstType);
            if (modeltype == null)
            {
                SpecialRender(context, ConstType, objectType, NameOrId, Parameters);
            }
            else
            {

                var repo = context.SiteDb.GetRepository(modeltype);

                ISiteObject siteobject = null;
                siteobject = repo?.GetByNameOrId(NameOrId) as ISiteObject;

                if (siteobject == null)
                {
                    long logid = -1;

                    if (long.TryParse(NameOrId, out logid))
                    {
                        var logentry = context.SiteDb.Log.Get(logid);
                        siteobject = repo.GetByLog(logentry) as ISiteObject;
                    }
                }

                if (siteobject != null)
                {
                    var contenttype = Service.ConstTypeService.GetContentType(ConstType);
                    context.RenderContext.Response.ContentType = contenttype;

                    if (siteobject is ITextObject)
                    {
                        context.RenderContext.Response.ContentType += ";charset=utf-8";
                        var textobject = siteobject as ITextObject;
                        context.RenderContext.Response.Body = DataConstants.DefaultEncoding.GetBytes(textobject.Body);
                    }
                    else if (siteobject is IBinaryFile)
                    {
                        var binary = siteobject as IBinaryFile;
                        context.RenderContext.Response.Body = binary.ContentBytes;
                    }
                    else
                    {
                        context.RenderContext.Response.ContentType += ";charset=utf-8";
                        var json = Lib.Helper.JsonHelper.Serialize(siteobject);
                        context.RenderContext.Response.Body = DataConstants.DefaultEncoding.GetBytes(json);
                    }
                }
            }
        }

        public static void SpecialRender(FrontContext context, byte ConstType, string strObjectType, string NameOrId, Dictionary<string, string> Parameters)
        {
            if (strObjectType != null && strObjectType.ToLower() == "kexternalcache")
            {
                Sites.Render.Renderers.ExternalCacheRender.Render(context, NameOrId);
            }
        }

        public static void KFileRender(FrontContext context, Dictionary<string, string> Parameters)
        {
            if (!context.RenderContext.WebSite.EnableFileIOUrl)
            {
                context.RenderContext.Response.StatusCode = 503;
                context.RenderContext.Response.End = true;
            }

            string relative = context.RenderContext.Request.RelativeUrl;

            string fullpath;

            fullpath = GetFileFullPath(context.RenderContext, relative);

            if (!string.IsNullOrWhiteSpace(fullpath))
            {
                string contentType = IOHelper.MimeType(relative);

                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = "application/octet-stream";
                }

                context.RenderContext.Response.ContentType = contentType;
                 

                if (contentType.ToLower().Contains("image"))
                {
                    var allbytes = Lib.Helper.IOHelper.ReadAllBytes(fullpath);

                    allbytes = setImageBytes(context, allbytes);
                    SetImageCache(context.RenderContext);

                    context.RenderContext.Response.Body = allbytes;

                } 
                else
                {
                    var fileinfo = new System.IO.FileInfo(fullpath);
                    if (fileinfo.Length > 1024 * 1024 * 3)
                    {
                        var stream = Kooboo.IndexedDB.StreamManager.OpenReadStream(fullpath);
                        context.RenderContext.Response.Stream = stream;
                    }
                    else
                    {
                        var allbytes = Lib.Helper.IOHelper.ReadAllBytes(fullpath);
                        context.RenderContext.Response.Body = allbytes;
                    } 
                } 
            }
        }

        public static string GetFileFullPath(RenderContext context, string relative)
        {
            string fullpath;
            relative = Lib.Helper.StringHelper.ReplaceIgnoreCase(relative, "__kb/kfile/", "");

            var root = Kooboo.Data.AppSettings.GetFileIORoot(context.WebSite);

            fullpath = Kooboo.Lib.Helper.IOHelper.CombinePath(root, relative);

            if (!System.IO.File.Exists(fullpath))
            {
                if (fullpath.Contains("?"))
                {
                    var markpos = fullpath.IndexOf("?");
                    fullpath = fullpath.Substring(0, markpos);
                }
            }

            if (System.IO.File.Exists(fullpath))
            {
                return fullpath; 
            }

            return null;  
        }

        public static byte[] setImageBytes(FrontContext context, byte[] currentbyes)
        { 
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
                        return Kooboo.Sites.Render.ImageRenderer.GetImageThumbnail(context.RenderContext, currentbyes, intwidth, intheight, 0);   
                        //Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImage(currentbyes, intwidth, intheight);
                    }
                } 
                else
                {
                    int intwidth = 0;

                    if (int.TryParse(width, out intwidth))
                    { 
                        return Kooboo.Sites.Render.ImageRenderer.GetImageThumbnail(context.RenderContext, currentbyes, intwidth, intwidth, 0); 
                         //return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImage(currentbyes, intwidth, intwidth);
                    }
                }
            }

            return currentbyes; 
        }

        public static void SetImageCache(RenderContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

            if (context.WebSite.EnableImageBrowserCache)
            {
                if (context.WebSite.ImageCacheDays > 0)
                {
                    context.Response.Headers["Expires"] = DateTime.UtcNow.AddDays(context.WebSite.ImageCacheDays).ToString("r");
                }
                else
                {
                    // double verify...
                    var version = context.Request.GetValue("version");
                    if (!string.IsNullOrWhiteSpace(version))
                    {
                        context.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
                    }
                }
            }

        }
    }
}