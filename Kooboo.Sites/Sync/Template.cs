using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Data.Template;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.TaskQueue.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Sites.Sync
{
    public static class Template
    {
        public static DateTime LastNotify { get; set; } = DateTime.Now.AddMinutes(-100); 

        public static string ImagePath { get; set; } = Path.Combine(Data.AppSettings.ThemeFolder, "img");

        public static object _locker = new object();

        public static bool IsDownloading { get; set; }

        private static long _version = -1;
        public static long Version
        {
            get
            {
                if (_version == -1)
                {
                    foreach (var item in Data.GlobalDb.Template.Store.ItemCollection())
                    {
                        if (item.EditVersion > _version)
                        {
                            _version = item.EditVersion;
                        }
                    }
                }
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        private static Dictionary<long, int> versionfailedtimes = new Dictionary<long, int>();

        private static string _myip;
        public static string MyIp
        {
            get
            {
                if (_myip == null)
                {
                    string url = Data.Helper.AccountUrlHelper.Template("myip");

                    var ip = Lib.Helper.HttpHelper.Get<string>(url);
                    if (!string.IsNullOrEmpty(ip))
                    {
                        var myip = System.Net.IPAddress.Parse(ip);
                        if (myip != null)
                        {
                            _myip = myip.ToString();
                        }
                    }
                }
                return _myip;
            }

        }

        public static Guid UploadIn(TemplateDataModel model)
        {
            TemplatePackage package = AddNewPackage(model);
                       
            GlobalDb.Template.AddOrUpdate(package);

            //if (AppSettings.IsTemplateServer && AppSettings.IsOnlineServer)
            //{ 
            //    if (Template.LastNotify < DateTime.Now.AddMinutes(-5))
            //    {
            //        var syncitem = new TemplateSyncModel();
            //        syncitem.PackageId = package.Id;
            //        TaskQueue.QueueManager.Add(syncitem);
            //    }
            //}

            return package.Id;
        }

        public static TemplatePackage AddNewPackage(TemplateDataModel model)
        {
            TemplatePackage package = new TemplatePackage();
            package.Name = model.Name;
            package.Link = model.Link;
            package.OrganizationId = model.OrganizationId;
            package.UserId = model.UserId;
            var user = Data.GlobalDb.Users.Get(model.UserId);
            if (user != null)
            {
                package.UserName = user.FullName;
            }
            package.LastModified = DateTime.UtcNow;
            package.Description = model.Description;

            package.Tags = model.Tags;
            package.Size = model.Bytes.Length;

            if (package.Size == 0)
            {
                throw new Exception("zero package size");
            }

            File.WriteAllBytes(package.FilePath, model.Bytes);

            var sitedb = ImportBinary(model.Bytes, package);

            if (model.Images.Count() == 0)
            {
                model.Images = GetSiteImages(sitedb);
                if (model.Images.Count() > 0)
                {
                    model.Images.First().IsDefault = true;
                }
            }

            Lib.Helper.IOHelper.EnsureDirectoryExists(ImagePath);

            foreach (var item in model.Images)
            {
                string relativeurl = Guid.NewGuid().ToString().Replace("-", "") + GetExtension(item.FileName);
                var filepath = Path.Combine(ImagePath, relativeurl);

                var bytes = Convert.FromBase64String(item.Base64);

                File.WriteAllBytes(filepath, bytes);

                package.Images.Add(relativeurl);
                if (item.IsDefault)
                {
                    package.ThumbNail = relativeurl;
                }
            }

            if (string.IsNullOrEmpty(package.ThumbNail) && package.Images.Count() > 0)
            {
                package.ThumbNail = package.Images.First();
            }

            return package;
        }

        public static bool Update(TemplateUpdateModel model)
        {
            var package = Kooboo.Data.GlobalDb.Template.Get(model.Id);

            if (package == null)
            {
                throw new Exception("Package not found");
            }

            bool HasBinaryChange = (model.Bytes != null && model.Bytes.Length > 0) || model.NewImages.Count > 0;

            UpdatePackage(model, package);

            if (AppSettings.IsTemplateServer && AppSettings.IsOnlineServer)
            {
                var url = Data.Helper.AccountUrlHelper.Template("AddOrUpdate");
                var template = Sync.Template.PackageToTemplate(package);
                var version = Lib.Helper.HttpHelper.Post<long>(url, Lib.Helper.JsonHelper.Serialize(template));
                if (version <= 0)
                {
                    throw new Exception("Template update failed");
                }
                else
                {
                    package.EditVersion = version;
                }
            } 

            GlobalDb.Template.AddOrUpdate(package);

            if (AppSettings.IsTemplateServer && AppSettings.IsOnlineServer)
            {
                if (Template.LastNotify < DateTime.Now.AddMinutes(-5))
                {
                    var syncitem = new TemplateSyncModel();
                    syncitem.PackageId = package.Id;
                    syncitem.HasBinaryChange = HasBinaryChange;
                    TaskQueue.QueueManager.Add(syncitem);
                }
            } 

            return true;
        }

        public static void UpdatePackage(TemplateUpdateModel model, TemplatePackage package)
        {
            if (model.Bytes != null && model.Bytes.Length > 0)
            {
                Guid oldsiteid = package.Id;
                // there is a change of zip.
                File.WriteAllBytes(package.FilePath, model.Bytes);
                // update to preview sites.
                ImportBinary(model.Bytes, package);
                // remove the old site. 
                Service.WebSiteService.Delete(oldsiteid);
            }
            package.Link = model.Link;
            package.Tags = model.Tags;
            package.Description = model.Description;
            package.OrganizationId = model.OrganizationId; 

            // now check if any images get deleted.  
            if (string.IsNullOrEmpty(model.Images))
            {
                package.Images.Clear();
            }
            else
            {
                List<string> imglist = Lib.Helper.JsonHelper.Deserialize<List<string>>(model.Images);

                package.Images = clearImagePath(package.Images, imglist);

                if (!string.IsNullOrEmpty(model.NewDefault))
                {     
                    if (int.TryParse(model.NewDefault, out int outnumber))
                    {
                        package.ThumbNail = package.Images[outnumber]; 
                    }
                    else
                    {    
                        package.ThumbNail = model.NewDefault;
                    }
                }
            }
             
            Lib.Helper.IOHelper.EnsureDirectoryExists(ImagePath);

            foreach (var item in model.NewImages)
            {
                string relativeurl = Guid.NewGuid().ToString().Replace("-", "") + GetExtension(item.FileName);
                var filepath = Path.Combine(ImagePath, relativeurl);
                var bytes = Convert.FromBase64String(item.Base64);

                File.WriteAllBytes(filepath, bytes);
                package.Images.Add(relativeurl);
                if (item.IsDefault)
                {
                    package.ThumbNail = relativeurl;
                }
            }

            if (package.Images.Count() > 0)
            {
                if (string.IsNullOrEmpty(package.ThumbNail) || !package.Images.Contains(package.ThumbNail))
                {
                    package.ThumbNail = package.Images.First();
                }
            }

        }

        private static List<string> clearImagePath(List<string> oldlist, List<string> newlist)
        {
            List<string> result = new List<string>();

            foreach (var item in oldlist)
            {
                string ImgFileName = item;
                int index = item.LastIndexOf('\\');
                if (index == -1)
                {
                    index = item.LastIndexOf('/');
                }
                if (index > -1)
                {
                    ImgFileName = item.Substring(index + 1);
                }

                foreach (var newitem in newlist)
                {
                    if (newitem.IndexOf(ImgFileName, StringComparison.OrdinalIgnoreCase) > -1)
                    { 
                        result.Add(ImgFileName);
                        break; 
                    }
                    
                }
            }

            return result;
        }
           
        public static void Delete(TemplatePackage package)
        {
            /// delete website. 
            GlobalDb.Template.Delete(package.Id);

            if (System.IO.File.Exists(package.FilePath))
            {
                System.IO.File.Delete(package.FilePath);
            } 
            foreach (var item in package.Images)
            {
                string fullimagefile = System.IO.Path.Combine(ImagePath, item);
                if (System.IO.File.Exists(fullimagefile))
                {
                    System.IO.File.Delete(fullimagefile);
                }
            } 
            Service.WebSiteService.Delete(package.SiteId); 
        }
         
        private static SiteDb ImportBinary(byte[] Bytes, TemplatePackage package)
        {
            MemoryStream filestream = new MemoryStream(Bytes);

            string newguid = package.Id.ToString().Replace("-", "");
            string hostname = newguid + "." + AppSettings.ThemeDomain;
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
                Service.WebSiteService.Delete(oldsite.Id);
            }

            GlobalDb.WebSites.AddOrUpdate(newsite);
            GlobalDb.Bindings.AddOrUpdate(hostname, newsite.Id, default(Guid));

            ImportExport.ImportZip(filestream, newsite);
            package.SiteId = newsite.Id;
            package.SiteName = newguid;
            var sitedb = newsite.SiteDb();
            package.PageCount = sitedb.Pages.Count();
            package.ContentCount = sitedb.TextContent.Count();
            package.ImageCount = sitedb.Images.Count();
            package.LayoutCount = sitedb.Layouts.Count();
            package.ViewCount = sitedb.Views.Count();
            package.MenuCount = sitedb.Menus.Count();

            return sitedb;
        }

        private static string GetExtension(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return ".png";
            }
            string ext = Kooboo.Lib.Helper.UrlHelper.FileExtension(input);

            if (string.IsNullOrEmpty(ext))
            {
                return ".png";
            }
            else
            {
                return ext;
            }
        }

        private static List<TemplateUserImages> GetSiteImages(SiteDb siteDb)
        {
            List<TemplateUserImages> result = new List<TemplateUserImages>();
            if (string.IsNullOrEmpty(AppSettings.ScreenShotUrl) || !AppSettings.IsOnlineServer)
            {
                var defaultimg = System.IO.Path.Combine(Data.AppSettings.RootPath, "_admin", "images", "default.png");
                if (System.IO.File.Exists(defaultimg))
                {
                    var bytes = File.ReadAllBytes(defaultimg);
                    TemplateUserImages image = new TemplateUserImages();
                    image.Base64 = Convert.ToBase64String(bytes);
                    image.FileName = Lib.Security.ShortGuid.GetNewShortId() + ".png";
                    result.Add(image);
                }
                return result;
            }

            var pageids = GetPageIds(siteDb);

            siteDb.WebSite.BaseUrl();

            int count = 0;

            foreach (var item in pageids)
            {
                var url = Service.ObjectService.GetObjectFullUrl(siteDb.WebSite, item);
                if (!string.IsNullOrEmpty(url) && !url.Contains("%") && !url.Contains("{"))
                {
                    var decodedurl = System.Net.WebUtility.UrlDecode(url);
                    if (!decodedurl.Contains("{"))
                    {
                        var base64image = Data.Helper.ChromeScreenShotHelper.GetScreenShot(url);
                        if (!string.IsNullOrEmpty(base64image))
                        {
                            TemplateUserImages image = new TemplateUserImages();
                            image.Base64 = base64image;
                            image.FileName = Lib.Security.ShortGuid.GetNewShortId() + ".png";
                            result.Add(image);
                            count += 1;
                            if (count >= 2)
                            {
                                return result;
                            }
                        }
                    }
                }
            }

            return result;
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

        public static Data.Models.Template PackageToTemplate(TemplatePackage package)
        {
            Data.Models.Template template = new Data.Models.Template();
            template.Id = package.Id;
            template.Description = package.Description;
            template.Link = package.Link;
            template.Name = package.Name;
            template.Tags = package.Tags;
            template.Thumbnail = package.ThumbNail;
            template.Images = string.Join(";", package.Images.ToArray());
            template.UserId = package.UserId;
            template.OrganizationId = package.OrganizationId;
            return template;
        }

        public static TemplatePackage TemplateToPackage(Data.Models.Template template)
        {
            TemplatePackage package = new TemplatePackage();

            package.Id = template.Id;
            package.Description = template.Description;
            package.Link = template.Link;
            package.Name = template.Name;
            package.Tags = template.Tags;
            package.ThumbNail = template.Thumbnail;
            if (!string.IsNullOrEmpty(template.Images))
            {
                package.Images = template.Images.Split(';').ToList();
            }
            package.UserId = template.UserId;
            package.OrganizationId = template.OrganizationId;

            package.EditVersion = template.Version;

            return package;
        }

        public static void DownloadSync()
        {
            if (!IsDownloading)
            {
                lock (_locker)
                {
                    if (!IsDownloading)
                    {
                        IsDownloading = true;
                        try
                        {
                            DownloadTemplate();
                        }
                        catch (Exception)
                        {
                            if (versionfailedtimes.ContainsKey(Version))
                            {
                                var times = versionfailedtimes[Version];
                                if (times > 3)
                                {
                                    Version = Version + 1;
                                    versionfailedtimes.Remove(Version);
                                }
                            }
                            else
                            {
                                versionfailedtimes.Add(Version, 1);
                                var olds = versionfailedtimes.Keys.Where(o => o < Version).ToList();
                                foreach (var item in olds)
                                {
                                    versionfailedtimes.Remove(item);
                                }
                            }
                        }

                        IsDownloading = false;
                    }
                }
            }
        }

        private static void DownloadTemplate()
        {
            string url = Data.Helper.AccountUrlHelper.Template("ListNext");
            url += "?version=" + Version.ToString();

            var list = Lib.Helper.HttpHelper.Get<List<Data.Models.Template>>(url);

            if (list == null || list.Count() == 0)
            {
                return;
            } 
            var serverips = Data.Helper.TemplateHelpder.SyncServerIps;
            List<string> otherips = new List<string>();
            foreach (var item in serverips)
            {
                if (!Lib.Helper.IPHelper.IsInSameCClass(MyIp, item))
                {
                    otherips.Add(item);
                }
            }

            foreach (var item in list.OrderBy(o => o.Version))
            { 
                if (item.IsDelete)
                {
                    var package = GlobalDb.Template.Get(item.Id);
                    if (package != null)
                    {
                        Delete(package);
                    }
                }

                else
                { 
                    var package = TemplateToPackage(item);
                    if (item.BinaryChange)
                    {
                        var binary = DownloadPackage(package.Id, otherips, item.OriginalIp);
                        if (binary == null || binary.Length == 0)
                        { return;  }
                        package.Size = binary.Length;
                        package.LastModified = DateTime.UtcNow;

                        Dictionary<string, byte[]> imagebytes = new Dictionary<string, byte[]>();

                        foreach (var img in package.Images)
                        {
                            var imgbytes = DownloadImage(img, otherips, item.OriginalIp);
                            if (imgbytes == null || imgbytes.Length == 0)
                            {
                                return;
                            }
                            imagebytes[img] = imgbytes;
                        }

                        ImportBinary(binary, package);

                        Lib.Helper.IOHelper.EnsureDirectoryExists(ImagePath);

                        foreach (var img in imagebytes)
                        {
                            var filepath = Path.Combine(ImagePath, img.Key);

                            File.WriteAllBytes(filepath, img.Value);
                        }

                        File.WriteAllBytes(package.FilePath, binary);
                    }

                    GlobalDb.Template.AddOrUpdate(package);
                }

                if (item.Version > Version)
                {
                    Version = item.Version;
                }
            }

            if (list.Count() >= 10)
            {
                DownloadTemplate();
            }
        }

        private static byte[] DownloadPackage(Guid PackageId, List<string> serverips, string orgip)
        {

            if (!Lib.Helper.IPHelper.IsInSameCClass(MyIp, orgip))
            {
                string orgurl = "http://" + orgip + "/_api/download/package/" + PackageId.ToString();
                var orgbytes = Kooboo.Lib.Helper.DownloadHelper.DownloadFile(orgurl, "zip");
                if (orgbytes != null && orgbytes.Length > 100)
                {
                    return orgbytes;
                }
            }

            foreach (var item in serverips)
            {
                string url = "http://" + item + "/_api/download/package/" + PackageId.ToString();

                var bytes = Lib.Helper.DownloadHelper.DownloadFile(url, "zip");
                if (bytes != null && bytes.Length > 0)
                {
                    return bytes;
                }
            }
            return null;
        }

        public static byte[] DownloadImage(string relativeurl, List<string> serverips, string orgip)
        {
            if (!Lib.Helper.IPHelper.IsInSameCClass(MyIp, orgip))
            {
                string orgurl = "http://" + orgip + "/_api/download/themeimg/" + relativeurl;
                var orgbytes = Lib.Helper.DownloadHelper.DownloadFile(orgurl, "image");
                if (orgbytes != null && orgbytes.Length > 20)
                {
                    return orgbytes;
                }
            }

            foreach (var item in serverips)
            {
                string url = "http://" + item + "/_api/download/themeimg/" + relativeurl;
                var bytes = Lib.Helper.DownloadHelper.DownloadFile(url, "image");
                if (bytes != null && bytes.Length > 0)
                {
                    return bytes;
                }
            }
            return null;
        }

        public static byte[] GetImage(string relativeUrl)
        {
            if (relativeUrl.StartsWith("/") || relativeUrl.StartsWith("\\"))
            {
                relativeUrl = relativeUrl.Substring(1);
            }
            var filepath = Path.Combine(ImagePath, relativeUrl);
            if (File.Exists(filepath))
            {
                return File.ReadAllBytes(filepath);
            }
            return null;
        }
    }
}
