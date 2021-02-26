using Kooboo.Data.Models;
using Kooboo.Sites.Render.Renderers.Custom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.LocalCache
{
    // cache remote resource into local disk folder. 
    public static class LocalCacheManager
    {
        static LocalCacheManager()
        {
            cachefiles = new Dictionary<string, LocalItem>();
            BaseDir = System.IO.Path.Combine(Kooboo.Data.AppSettings.TempDataPath, "localcache");
        }

        internal static string BaseDir
        {
            get; set;
        }

        internal static Dictionary<string, LocalItem> cachefiles { get; set; }
         
        public static string SetItem(WebSite site, string fullurl, int hours = 3)
        {
            string name = Kooboo.Lib.Helper.UrlHelper.GetNameFromUrl(fullurl);

            string localname = GetSiteLocalName(site, name);
            if (!cachefiles.ContainsKey(localname))
            {
                var item = new LocalItem();
                item.OldFullUrl = fullurl;
                item.LastModify = DateTime.Now;
                item.LocalPath = name;
                item.CacheHours = hours;
                cachefiles[localname] = item;
                return name;
            }
            else
            {
                var rightname = GetRightName(site, name, fullurl);
                if (rightname != name)
                {
                    var localrightname = GetSiteLocalName(site, rightname);
                    var item = new LocalItem();
                    item.OldFullUrl = fullurl;
                    item.LastModify = DateTime.Now;
                    item.CacheHours = hours;
                    item.LocalPath = rightname;
                    cachefiles[localrightname] = item;
                }
                return rightname;
            }
        }


        public static string GetUrl(string itemPath)
        {
            return  CustomRenderManager.GetRoute<LocalCacheCustomRender>(itemPath);
        }

        public static async Task<byte[]> GetItem(WebSite site, string Path)
        {
            var localname = GetSiteLocalName(site, Path);

            if (cachefiles.ContainsKey(localname))
            {
                var item = cachefiles[localname];

                var diskpath = GetDiskPath(site, Path);

                byte[] result = null;
                if (System.IO.File.Exists(diskpath))
                {
                    result = System.IO.File.ReadAllBytes(diskpath);
                }

                if (item != null)
                {
                    if (result != null)
                    {
                        if (item.LastModify < DateTime.Now.AddHours(item.CacheHours))
                        {
                            FetchItem(site, item);
                        }
                        return result;
                    }
                    else
                    {
                        await FetchItem(site, item);
                        if (System.IO.File.Exists(diskpath))
                        {
                            return System.IO.File.ReadAllBytes(diskpath); 
                        }
                    }
                }
            }
            return null;
        }

        internal static string GetSiteLocalName(WebSite site, string name)
        {
            return site.Name + "/" + name;
        }

        internal static string GetRightName(WebSite site, string name, string fullUrl)
        {
            var localname = GetSiteLocalName(site, name);

            if (!cachefiles.ContainsKey(localname))
            {
                return name;
            }

            var item = cachefiles[localname];

            var newname = name;
            var newlocalname = newname; 

            for (int i = 1; i < 9999; i++)
            {
                if (item != null && item.OldFullUrl.ToLower() != fullUrl.ToLower())
                {
                    newname = i.ToString() + name; 
                    newlocalname = GetSiteLocalName(site, newname);
                    if (!cachefiles.ContainsKey(newlocalname))
                    {
                        return newname;
                    }
                    else
                    {
                        item = cachefiles[newlocalname];
                    }
                }
                else
                {
                    return item.LocalPath;
                }
            }
            return newname;
        }
  
        internal static string GetDiskPath(WebSite site, string path)
        {
            //var localname = GetSiteLocalName(site, path); 
            return System.IO.Path.Combine(BaseDir, site.Name, path);
        }

        internal static async Task FetchItem(WebSite site, LocalItem item)
        {
            var download = await Kooboo.Lib.Helper.DownloadHelper.DownloadFileAsync(item.OldFullUrl, null, null);
            SetLocalDisk(site, item, download); 
        }

        internal static void SetLocalDisk(WebSite site, LocalItem item, byte[] bytes)
        {
            if (bytes != null)
            {
                var diskpath = GetDiskPath(site, item.LocalPath);
                Lib.Helper.IOHelper.EnsureFileDirectoryExists(diskpath); 
                System.IO.File.WriteAllBytes(diskpath, bytes);
                item.LastModify = DateTime.Now;
            }
        }
         
    }

    public class LocalItem
    {
        public string LocalPath { get; set; }

        public string OldFullUrl { get; set; }

        // a time of pass to enable fetch directly. 
        public DateTime LastModify { get; set; } = DateTime.Now.AddYears(-10);

        public int CacheHours { get; set; } = 3;

        public Guid SiteId { get; set; }
    }
}
