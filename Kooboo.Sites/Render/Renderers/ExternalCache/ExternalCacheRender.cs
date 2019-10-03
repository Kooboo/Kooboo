using System;
using System.Threading;
using Kooboo.Sites.Render.Renderers.ExternalCache;

namespace Kooboo.Sites.Render.Renderers
{
    public static class ExternalCacheRender
    {
        public static void Render(FrontContext context, string NameOrId)
        {
            var item = GetItem(NameOrId);
            if (item != null)
            {
                var binary = GetBinary(item);
                if (binary != null)
                {
                    var contenttype = item.ContentType;
                    if (string.IsNullOrWhiteSpace(contenttype))
                    {
                        contenttype = "application/octet-stream";
                    }
                    context.RenderContext.Response.ContentType = item.ContentType;

                    if (!string.IsNullOrWhiteSpace(item.name))
                    {
                        context.RenderContext.Response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(item.name)}");
                    } 

                    context.RenderContext.Response.Body = binary;

                    context.RenderContext.Response.End = true; 
                }
            }
        }

        public static CacheObject GetItem(string shortguid)
        {
            if (string.IsNullOrWhiteSpace(shortguid))
            {
                return null;
            }

            var path = DiskPathJson(shortguid);

            if (System.IO.File.Exists(path))
            {
                var text = System.IO.File.ReadAllText(path);

                try
                {
                    return Lib.Helper.JsonHelper.Deserialize<CacheObject>(text);
                }
                catch (Exception ex)
                {
                    Kooboo.Data.Log.Instance.Exception.Write(ex.Message + ex.Source + ex.StackTrace);
                }
            }
            return null;

        }

        public static byte[] GetBinary(CacheObject item)
        {
            string binarypath = DiskPathBinary(item.Id);
            if (!System.IO.File.Exists(binarypath) || item.Expiration < DateTime.Now)
            {
                var down = Lib.Helper.DownloadHelper.DownloadUrl(item.FullFileUrl);
                if (down != null)
                {
                    if (!string.IsNullOrEmpty(down.ContentType))
                    {
                        CheckOrUpdateContentType(item, down.ContentType);
                    }
                    SaveBinaryDisk(item, down.DataBytes);

                    return down.DataBytes;
                }
            }

            if (System.IO.File.Exists(binarypath))
            {
                return System.IO.File.ReadAllBytes(binarypath);
            }
            return null;
        }


        public static string AddNew(string fullurl, int interval)
        {
            CacheObject item = new CacheObject(fullurl, null, null, interval);
            SaveDiskJson(item);
            StartNewDownload(item);
            return Lib.Security.ShortGuid.Encode(item.Id);
        }

        public static void SaveDiskJson(CacheObject item)
        {
            var path = DiskPathJson(item.Id);
            var text = Lib.Helper.JsonHelper.Serialize(item);
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(path);
            System.IO.File.WriteAllText(path, text);
        }

        public static void CheckOrUpdateContentType(CacheObject item, string contenttype)
        {
            if (!string.IsNullOrWhiteSpace(contenttype) && !Lib.Helper.StringHelper.IsSameValue(contenttype, item.ContentType))
            {
                item.ContentType = contenttype;
                SaveDiskJson(item);
            }
        }

        public static void SaveBinaryDisk(CacheObject item, byte[] Binary)
        {
            if (Binary == null)
            {
                return;
            }
            var binarypath = DiskPathBinary(item.Id);

            if (System.IO.File.Exists(binarypath))
            {
                var allbytes = System.IO.File.ReadAllBytes(binarypath);
                if (allbytes != null)
                {
                    var oldhash = Lib.Security.Hash.ComputeGuid(allbytes);
                    var newhash = Lib.Security.Hash.ComputeGuid(Binary);
                    if (oldhash != newhash)
                    {
                        System.IO.File.WriteAllBytes(binarypath, Binary);
                    }
                }
            }
            else
            {
                Lib.Helper.IOHelper.EnsureFileDirectoryExists(binarypath);
                System.IO.File.WriteAllBytes(binarypath, Binary);
            }

            item.Expiration = DateTime.Now.AddSeconds(item.interval);
            SaveDiskJson(item);
        }

        private static string CacheFolder
        {
            get
            {
                return System.IO.Path.Combine(Data.AppSettings.RootPath, "AppData", "Cache");
            }
        }

        public static string DiskPathJson(Guid id)
        {
            var strid = Lib.Security.ShortGuid.Encode(id);
            return DiskPathJson(strid);
        }

        public static string DiskPathJson(string shortGuid)
        {
            return System.IO.Path.Combine(CacheFolder, "Json", shortGuid);
        }

        public static string DiskPathBinary(Guid id)
        {
            var strid = Lib.Security.ShortGuid.Encode(id);

            return System.IO.Path.Combine(CacheFolder, "Binary", strid);
        }

        public static void StartNewDownload(CacheObject item)
        {
            var work = new CacheBinaryDownloader(item);
            var newThread = new Thread(work.DownloadAndSave);
            newThread.Start();
        }

    }


}
