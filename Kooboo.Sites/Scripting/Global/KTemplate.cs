using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Repository;
using System.IO;
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Sites.Sync;
using Kooboo.Sites.Routing;
using Kooboo.Lib.Compatible;
using Kooboo.Lib.Helper;
using Kooboo.Data.Template;
using KScript;

namespace Kooboo.Sites.Scripting.Global
{
    public class KTemplate
    {
        public static string ImagePath { get; set; } = Path.Combine(Data.AppSettings.ThemeFolder, "img");
        public RenderContext _context;
        public KTemplate(RenderContext context)
        {
            _context = context;
        }

        public string GetTemplateId(byte[] postData)
        {
            IndexedDB.Serializer.Simple.SimpleConverter<TemplateUpdateModel> converter = new IndexedDB.Serializer.Simple.SimpleConverter<TemplateUpdateModel>();
            var model = converter.FromBytes(postData);
            return model != null ? model.Id.ToString() : "";
        }
        public Dictionary<string,object> UpdateTemplate(string domain,byte[] postData, DynamicTableObject obj)
        {
            var oldTemplate = obj.Values;
            IndexedDB.Serializer.Simple.SimpleConverter<TemplateUpdateModel> converter = new IndexedDB.Serializer.Simple.SimpleConverter<TemplateUpdateModel>();
            var model = converter.FromBytes(postData);
            var hash = Lib.Security.Hash.ComputeGuid(model.Bytes);

            var userId = oldTemplate["userId"];
            if (hash != model.ByteHash ||
                model.UserId.ToString() != userId.ToString())
            {
                return null;
            }

            oldTemplate["link"] = model.Link;
            oldTemplate["description"] = model.Description;
            oldTemplate["tags"] = model.Tags;
            oldTemplate["lastModified"] = DateTime.UtcNow;
            oldTemplate["lastModifiedTimeStamp"] = DateTime.UtcNow.Ticks;

            var existImages = new List<string>();
            if (oldTemplate["images"] != null && !string.IsNullOrEmpty(oldTemplate["images"].ToString()))
            {
                var oldlist = Kooboo.Lib.Helper.JsonHelper.Deserialize<List<string>>(oldTemplate["images"].ToString());
                var newlist = JsonHelper.Deserialize<List<string>>(model.Images);
                existImages = clearImagePath(oldlist, newlist);
            }

            var images = new List<ScreenshotImage>();
            var thumbnail = string.Empty;

            if (model.Bytes!=null && model.Bytes.Length > 0)
            {
                Guid oldsiteid = model.Id;
                // there is a change of zip.
                File.WriteAllBytes(GetFilePath(model.Id.ToString()), model.Bytes);
                oldTemplate["size"] = model.Bytes.Length;
                // update to preview sites.
                var siteDb= ImportBinary(domain,model.Bytes, oldTemplate);

                if (model.NewImages.Count == 0 && existImages.Count()==0)
                {
                    images = GetScreenshotImages(siteDb,domain);
                    if (images.Count > 0)
                    {
                        thumbnail = images[0].FileName;
                    }
                }
                // remove the old site. 
                Sites.Service.WebSiteService.Delete(oldsiteid);
            }
            foreach (var image in model.NewImages)
            {
                var screenshotImage = new ScreenshotImage
                {
                    Base64 = image.Base64,
                    FileName = Guid.NewGuid().ToString().Replace("-", "") + Kooboo.Lib.Helper.UrlHelper.FileExtension(image.FileName)
                };
                images.Add(screenshotImage);
                if (image.IsDefault)
                {
                    thumbnail = screenshotImage.FileName;
                }
            }

            var imagelist = new List<string>();
            foreach (var image in images)
            {
                string relativeurl = image.FileName;
                var filepath = GetImagePath(relativeurl);

                File.WriteAllBytes(filepath, image.Bytes);
                imagelist.Add(relativeurl);
            }
            existImages.AddRange(imagelist);
            oldTemplate["images"] = existImages.ToArray();
            oldTemplate["thumbNail"] = string.IsNullOrEmpty(thumbnail) ? GetDefaultImage(existImages, model.NewDefault) : thumbnail;
            return oldTemplate;

        }

        public Dictionary<string,object> UploadOldFormat(string domain,byte[] postData)
        {
            IndexedDB.Serializer.Simple.SimpleConverter<TemplateDataModel> converter = new IndexedDB.Serializer.Simple.SimpleConverter<TemplateDataModel>();
            var model = converter.FromBytes(postData);
            var hash = Lib.Security.Hash.ComputeGuid(model.Bytes);
            if (hash != model.ByteHash||
                model.UserId == default(Guid)|| model.Bytes.Length==0)
            {
                return null;
            }

            var dic = new Dictionary<string, object>();
            dic["name"] = model.Name;
            dic["link"] = model.Link;
            dic["description"] = model.Description;
            dic["tags"] = model.Tags;
            dic["userId"] = model.UserId;
            var user = Data.GlobalDb.Users.Get(model.UserId);
            if (user != null)
            {
                dic["userName"] = user.FullName;
            }

            dic["lastModified"] = DateTime.UtcNow;
            dic["lastModifiedTimeStamp"] = DateTime.UtcNow.Ticks;
            dic["downloadCount"] = 0;
            dic["score"] = 0;
            dic["id"] = Guid.NewGuid().ToString();

            var filePath = GetFilePath(dic["id"].ToString());
            File.WriteAllBytes(filePath, model.Bytes);
            dic["size"] = model.Bytes.Length;

            var siteDb = ImportBinary(domain, model.Bytes, dic);
            Lib.Helper.IOHelper.EnsureDirectoryExists(ImagePath);

            var images = new List<ScreenshotImage>();
            var thumbnail = "";
            if (model.Images.Count() == 0)
            {
                images = GetScreenshotImages(siteDb,domain);
                if (images.Count > 0)
                {
                    thumbnail = images[0].FileName;
                }
            }
            else
            {
                foreach (var image in model.Images)
                {
                    var screenshotImage = new ScreenshotImage
                    {
                        Base64 = image.Base64,
                        FileName = Guid.NewGuid().ToString().Replace("-", "") + Kooboo.Lib.Helper.UrlHelper.FileExtension(image.FileName)
                    };
                    images.Add(screenshotImage);
                    if (image.IsDefault)
                    {
                        thumbnail = screenshotImage.FileName;
                    }
                }
                if(string.IsNullOrEmpty(thumbnail) && images.Count > 0)
                {
                    thumbnail = images[0].FileName;
                }
            }

            var imagelist = new List<string>();
            foreach (var image in images)
            {
                string relativeurl = image.FileName;
                var filepath = GetImagePath(relativeurl);

                File.WriteAllBytes(filepath, image.Bytes);
                imagelist.Add(relativeurl);
            }
            dic["images"] = imagelist.ToArray();
            dic["thumbNail"] = thumbnail;

            return dic;
        }
        public Dictionary<string, object> Upload(string domain, byte[] postdata)
        {
            var dic = new Dictionary<string, object>();
            var formResult = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(postdata);

            if (formResult.FormData.ContainsKey("title"))
            {
                dic["name"] = formResult.FormData["title"];
            }

            if (formResult.FormData.ContainsKey("link"))
            {
                dic["link"] = formResult.FormData["link"];
            }
            if (formResult.FormData.ContainsKey("description"))
            {
                dic["description"] = formResult.FormData["description"];
            }
            if (formResult.FormData.ContainsKey("tags"))
            {
                dic["tags"] = formResult.FormData["tags"];
            }
            dic["lastModified"] = DateTime.UtcNow;
            dic["lastModifiedTimeStamp"] = DateTime.UtcNow.Ticks;
            dic["downloadCount"] = 0;
            dic["score"] = 0;
            dic["id"] = Guid.NewGuid().ToString();

            if (formResult.Files.Count() > 0)
            {
                var zipfile = formResult.Files.Find(f => System.IO.Path.GetExtension(f.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase));
                if (zipfile == null)
                {
                    throw new Exception("no zip");
                }

                var filePath = GetFilePath(dic["id"].ToString());
                File.WriteAllBytes(filePath, zipfile.Bytes);
                //dic["filePath"] = filePath;
                dic["size"] = zipfile.Bytes.Length;

                var siteDb = ImportBinary(domain, zipfile.Bytes, dic);
                Lib.Helper.IOHelper.EnsureDirectoryExists(ImagePath);
                var uploadImages = formResult.Files.FindAll(f => Kooboo.Lib.Helper.UrlHelper.IsImage(f.FileName));

                var images = new List<ScreenshotImage>();
                if (uploadImages.Count == 0)
                {
                    images = GetScreenshotImages(siteDb,domain);
                }
                else
                {
                    foreach(var image in uploadImages)
                    {
                        images.Add(new ScreenshotImage
                        {
                            Bytes= image.Bytes,
                            FileName= Guid.NewGuid().ToString().Replace("-", "") + Kooboo.Lib.Helper.UrlHelper.FileExtension(image.FileName)
                        });
                    }
                }

                var imagelist = new List<string>();
                foreach (var image in images)
                {
                    string relativeurl = image.FileName;
                    var filepath = GetImagePath(relativeurl);

                    File.WriteAllBytes(filepath, image.Bytes);
                    imagelist.Add(relativeurl);
                }
                dic["images"] = imagelist.ToArray();
                dic["thumbNail"] = imagelist.Count > 0 ? imagelist[0] : "";
            }

            return dic;
        }

        private string GetDefaultImage(List<string> images,string newDefault)
        {
            if (images.Count > 0)
            {
                if (!string.IsNullOrEmpty(newDefault))
                {
                    if (int.TryParse(newDefault, out var index))
                    {
                        if (index < images.Count())
                        {
                            return images[index];
                            
                        }
                    }
                    return newDefault;
                }

                return images[0];
            }

            return string.Empty;
        }
        /// <summary>
        /// get template from old template server
        /// can be removed after all template is imported
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Dictionary<string,object> GetTemplateFromRemote(string domain, System.Dynamic.ExpandoObject obj)
        {
            var result = new Dictionary<string, object>();
            result["lastModified"] = DateTime.UtcNow;
            result["lastModifiedTimeStamp"] = DateTime.UtcNow.Ticks;

            var dic=obj as IDictionary<string,object>;
            var id =Guid.Parse(dic["id"].ToString());

            //get download package
            var packageurl = "http://47.52.131.96/_api/download/package?id=" + id.ToString("N");
            var bytes = Kooboo.Lib.Helper.HttpHelper.ConvertKooboo(packageurl, new byte[0], new Dictionary<string, string>());
            var filePath = GetFilePath(id.ToString());
            File.WriteAllBytes(filePath, bytes);

            //import site
            var siteDb = ImportBinary(domain, bytes, result);
            Lib.Helper.IOHelper.EnsureDirectoryExists(ImagePath);

            //download images
            var imageStr = dic["images"].ToString();
            var images = JsonHelper.Deserialize<List<string>>(imageStr);
            foreach(var image in images)
            {
                var imageUrl= "http://47.52.131.96/_api/download/themeimg/" + image;

                bytes= Kooboo.Lib.Helper.HttpHelper.ConvertKooboo(imageUrl, new byte[0], new Dictionary<string, string>());
                var filepath = GetImagePath(image);
                File.WriteAllBytes(filepath, bytes);
            }

            return result;
        }

        public void DeleteTemplate(DynamicTableObject obj)
        {
            var oldtemplate = obj.Values;
            var filePath = GetFilePath(oldtemplate["id"].ToString());
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            var images = JsonHelper.Deserialize<List<string>>(oldtemplate["images"].ToString());

            foreach (var item in images)
            {
                string fullimagefile = System.IO.Path.Combine(ImagePath, item);
                if (System.IO.File.Exists(fullimagefile))
                {
                    System.IO.File.Delete(fullimagefile);
                }
            }

            Sites.Service.WebSiteService.Delete(Guid.Parse(oldtemplate["siteId"].ToString()));
        }
        private List<ScreenshotImage> GetScreenshotImages(SiteDb siteDb,string domain)
        {
            var pageids = GetPageIds(siteDb);

            var images = new List<ScreenshotImage>();
            foreach (var item in pageids)
            {
                //var url = Kooboo.Sites.Service.ObjectService.GetObjectFullUrl(siteDb.WebSite, item);
                var url = GetObjectFullUrl(siteDb.WebSite, item, domain);
                if (!string.IsNullOrEmpty(url) && !url.Contains("%") && !url.Contains("{"))
                {
                    var decodedurl = System.Net.WebUtility.UrlDecode(url);
                    if (!decodedurl.Contains("{"))
                    {
                        var base64image = GetScreenShot(url);
                        if (!string.IsNullOrEmpty(base64image))
                        {
                            images.Add(new ScreenshotImage
                            {
                                Base64 = base64image,
                                FileName = Lib.Security.ShortGuid.GetNewShortId() + ".png"
                            });
                            
                            if (images.Count >= 2)
                            {
                                return images;
                            }
                        }
                    }
                }
            }

            return images;
        }

        private string GetObjectFullUrl(WebSite website, Guid ObjectId,string domain)
        {
           
            var baseurl = GetBaseUrl(website, domain);
            string objectUrl = website.SiteDb().Routes.GetObjectPrimaryRelativeUrl(ObjectId);
            if (!string.IsNullOrEmpty(objectUrl))
            {
                objectUrl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, objectUrl);
            }
            return objectUrl;
        }

        private static List<string> clearImagePath(List<string> oldlist, List<string> newlist)
        {
            List<string> result = new List<string>();

            foreach(var item in oldlist)
            {
                var exist = newlist.Exists(i => i.IndexOf(item) > -1);
                if (exist)
                {
                    result.Add(item);
                }
            }

            return result;
        }
        private string GetScreenShot(string url, int width = 600, int height = 450)
        {
            var screenshoturl = "http://sslgenerator.com/_api/screenshot/get";

            url = System.Net.WebUtility.UrlEncode(url);
            var nodeScreenShotUrl = string.Format("{0}?url={1}&width={2}&height={3}", screenshoturl, url, width, height);
            try
            {
                var base64Image = HttpHelper.Get<string>(nodeScreenShotUrl);
                base64Image = CompatibleManager.Instance.Framework.GetThumbnailImage(base64Image, new ImageSize() { Width = width, Height = height });
                // should verify as base64 here.. 
                return base64Image;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string GetPreviewUrl(string siteid,string domain)
        {
            var site = GlobalDb.WebSites.Get(Guid.Parse(siteid));
            //string baseurl = site.BaseUrl();

            var hostname =string.Format("{0}.{1}",site.Name,domain);
            var baseurl = GetBaseUrl(site, domain);
            return Lib.Helper.UrlHelper.Combine(baseurl, GetStartRelativeUrl(site));
        }

        private string GetBaseUrl(WebSite site,string domain)
        {
            //string baseurl = website.BaseUrl();
            var hostname = string.Format("{0}.{1}", site.Name, domain);
            var baseurl = string.Empty;
            if (site.ForceSSL)
            {
                var uri = new UriBuilder("https", hostname, 443);
                baseurl = uri.Uri.AbsoluteUri;
            }
            else
            {
                var uri = new UriBuilder("http", hostname, 80);
                baseurl = uri.Uri.AbsoluteUri;
            }
            return baseurl;
        }
        /// <summary>
        /// download site package
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public byte[] Download(string id)
        {
            var filePath = GetFilePath(id);
            var allbytes = new Byte[0];
            if (File.Exists(filePath))
            {
                allbytes = System.IO.File.ReadAllBytes(filePath);
            }

            return allbytes;
        }

        /// <summary>
        /// get site image
        /// </summary>
        /// <param name="imagename"></param>
        /// <returns></returns>
        public byte[] GetImage(string imagename)
        {
            return GetImage(imagename,0,0);
        }
        public byte[] GetImage(string imagename, int width, int height)
        {
            var path = GetImagePath(imagename);
            var allbytes = new Byte[0];
            if (File.Exists(path))
            {
                if (width > 0 || height > 0)
                {
                    allbytes = ThumbNail(imagename, width, height);
                }
                else
                {
                    allbytes = System.IO.File.ReadAllBytes(path);
                }

            }

            return allbytes;
        }

        private string GetFilePath(string id)
        {
            string file = id.Replace("-", "") + ".zip";
            string fullfilename = System.IO.Path.Combine(Data.AppSettings.ThemeFolder, "package", file);
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(fullfilename);
            return fullfilename;
        }

        private string GetImagePath(string imageName)
        {
            var filepath = Path.Combine(ImagePath, imageName);
            return filepath;
        }

        private byte[] ThumbNail(string name, int width, int height)
        {
            var path = System.IO.Path.Combine(Data.AppSettings.ThemeFolder, "img", name);

            string thumbnailname = width.ToString() + "_" + height.ToString() + "_" + name;

            string thumbfolder = System.IO.Path.Combine(Data.AppSettings.ThemeFolder, "thumbnail");
            Lib.Helper.IOHelper.EnsureDirectoryExists(thumbfolder);
            string thumbnailpath = System.IO.Path.Combine(thumbfolder, thumbnailname);

            if (!System.IO.File.Exists(thumbnailpath))
            {
                if (System.IO.File.Exists(path))
                {
                    var allbytes = System.IO.File.ReadAllBytes(path);

                    var image = CompatibleManager.Instance.Framework.GetImageSize(allbytes);

                    if (image.Height < height && image.Width < width)
                    {
                        return allbytes;
                    }

                    if (height <= 0 && image.Width > 0)
                    {
                        height = (int)width * image.Height / image.Width;
                    }

                    if (width <= 0 && image.Height > 0)
                    {
                        width = (int)height * image.Width / image.Height;
                    }
                    CompatibleManager.Instance.Framework.SaveThumbnailImage(allbytes, width, height, thumbnailpath);

                }
            }

            if (System.IO.File.Exists(thumbnailpath))
            {
                return System.IO.File.ReadAllBytes(thumbnailpath);
            }
            return new byte[0];
        }
        private SiteDb ImportBinary(string domain, byte[] Bytes, Dictionary<string, object> dic)
        {
            MemoryStream filestream = new MemoryStream(Bytes);

            string newguid = Guid.NewGuid().ToString().Replace("-", "");
            string hostname = newguid + "." + domain;
            WebSite newsite = new WebSite
            {
                Name = newguid,
                OrganizationId = default(Guid),
                EnableFrontEvents = false,
                EnableVisitorLog = false,
                EnableConstraintFixOnSave = false,
                EnableImageLog = false
            };

            var oldsite = Data.GlobalDb.WebSites.Get(newsite.Id);
            if (oldsite != null)
            {
                Sites.Service.WebSiteService.Delete(oldsite.Id);
            }

            GlobalDb.WebSites.AddOrUpdate(newsite);
            GlobalDb.Bindings.AddOrUpdate(hostname, newsite.Id, default(Guid));

            ImportExport.ImportZip(filestream, newsite);
            dic["siteId"] = newsite.Id;
            dic["siteName"] = newguid;
            var sitedb = newsite.SiteDb();
            dic["pageCount"] = sitedb.Pages.Count();
            dic["contentCount"] = sitedb.TextContent.Count();
            dic["imageCount"] = sitedb.Images.Count();
            dic["layoutCount"] = sitedb.Layouts.Count();
            dic["viewCount"] = sitedb.Views.Count();
            dic["menuCount"] = sitedb.Menus.Count();

            return sitedb;
        }

        private string GetStartRelativeUrl(Data.Models.WebSite site)
        {
            var startpages = site.StartPages();
            if (startpages != null && startpages.Count() > 0)
            {
                foreach (var item in startpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            var allpages = site.SiteDb().Pages.All();

            if (allpages != null && allpages.Count() > 0)
            {
                foreach (var item in allpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            if (allpages != null && allpages.Count() > 0)
            {
                foreach (var item in allpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null)
                    {
                        return route.Name;
                    }
                }
            }

            return "/";
        }

        private static HashSet<Guid> GetPageIds(SiteDb sitedb)
        {
            HashSet<Guid> Pageids = new HashSet<Guid>();

            var starts = sitedb.WebSite.StartPages();
            if (starts != null)
            {
                foreach (var item in starts)
                {
                    Pageids.Add(item.Id);
                }
            }
            if (Pageids.Count >= 10)
            {
                return Pageids;
            }

            foreach (var item in sitedb.Pages.All(true))
            {
                Pageids.Add(item.Id);
                if (Pageids.Count() >= 10)
                {
                    return Pageids;
                }
            }

            return Pageids;
        }
    }

    public class ScreenshotImage
    {
        public string FileName { get; set; }

        public string Base64 { get; set; }

        private byte[] _bytes;
        public byte[] Bytes
        {
            get
            {
                if (_bytes == null)
                {
                    _bytes= Convert.FromBase64String(Base64);
                }
                return _bytes;
            }
            set
            {
                _bytes = value;
            }
        }
        
    }
}
