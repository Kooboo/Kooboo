using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Kooboo.Sites.Render.Renderers
{
    public static class ExternalCacheRender
    {

        public static Dictionary<Guid, CacheObject> objects { get; set; }

        static ExternalCacheRender()
        {
            objects = new Dictionary<Guid, CacheObject>();
        }

        public static void Render(FrontContext context, string NameOrId)
        {
            var item = GetItem(NameOrId);
            if (item != null)
            {
                EnsureDownload(item).Wait();
                context.RenderContext.Response.ContentType = item.ContentType;
                context.RenderContext.Response.Body = item.Binary;
            }
        }

        public static async Task EnsureDownload(CacheObject item)
        {
            if (item.Binary == null)
            {
                if (item.ContentType == null)
                {
                    var down = await Lib.Helper.DownloadHelper.DownloadUrlAsync(item.FullFileUrl);
                    if (down != null)
                    {
                        item.Binary = down.DataBytes;
                        item.ContentType = down.ContentType;
                        if (item.ContentType == null)
                        {
                            if (item.name != null)
                            {
                                item.ContentType = Lib.Helper.IOHelper.MimeType(item.name);
                            }
                        }
                        if (item.ContentType == null)
                        {
                            item.ContentType = "application/octet-stream";
                        }
                    }
                }
                else
                {
                    var down = await Lib.Helper.DownloadHelper.DownloadFileAsync(item.FullFileUrl);
                    if (down != null)
                    {
                        item.Binary = down;
                    }
                }

            }
        }

        public static void AddOrUpdate(CacheObject item)
        {
            objects[item.Id] = item;
        }

        public static CacheObject GetItem(string shortStringId)
        {
            var guid = Lib.Security.ShortGuid.Decode(shortStringId);

            if (objects.ContainsKey(guid))
            {
                return objects[guid];
            }
            return null;
        }

        public static List<CacheObject> GetRequireUpdateList()
        {
            return objects.Values.ToList().Where(o => o.Expiration < DateTime.Now).ToList();
        }

        public static string AddNew(string fullurl, int interval)
        {
            CacheObject item = new CacheObject(fullurl, null, null, interval); 
            AddOrUpdate(item);
            StartNewDownload(item); 
            var id = item.Id; 
            return Lib.Security.ShortGuid.Encode(id);     
        }

        public static void StartNewDownload(CacheObject item)
        {
            var work = new CacheDownloader(item);
            var newThread = new Thread(work.DownloadAndSave);
            newThread.Start();
        }

    }

    public class CacheObject
    {

        public CacheObject(string fullurl, string name, string contenttype, int interval)
        {
            this.FullFileUrl = fullurl;
            this.name = name;
            this.ContentType = contenttype;
            this.interval = interval;

            this.Expiration = DateTime.Now.AddSeconds(this.interval);
        }


        private byte[] _binary;
        public byte[] Binary
        {
            get
            {
                return _binary;
            }
            set
            {
                _binary = value;
                this.Expiration = DateTime.Now.AddSeconds(interval);
            }

        }

        // seconds. 
        public int interval { get; set; }

        public DateTime Expiration { get; set; }

        public string ContentType { get; set; }
 

        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (FullFileUrl != null)
                    {
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(FullFileUrl);
                    }
                }
                return _id;
            }
        }

        public string FullFileUrl { get; set; }
  
        private string _name;
        public string name
        {
            get
            {
                if (_name == null && FullFileUrl != null)
                {
                    return Lib.Helper.UrlHelper.GetNameFromUrl(FullFileUrl);
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

    }


    public class CleanCache : IBackgroundWorker
    {
        public int Interval => 60 * 5;

        public DateTime LastExecute { get; set; }

        public void Execute()
        {
            var list = ExternalCacheRender.GetRequireUpdateList();
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    ExternalCacheRender.StartNewDownload(item);
                }
            }
        }
    }

    public class CacheDownloader
    {
        public CacheDownloader(CacheObject item)
        {
            this.item = item;
        }

        private CacheObject item { get; set; }


        public void DownloadAndSave()
        {

            if (item.ContentType == null)
            {
                var down = Lib.Helper.DownloadHelper.DownloadUrl(item.FullFileUrl);
                if (down != null)
                {
                    item.Binary = down.DataBytes;
                    item.ContentType = down.ContentType;
                    if (item.ContentType == null)
                    {
                        if (item.name != null)
                        {
                            item.ContentType = Lib.Helper.IOHelper.MimeType(item.name);
                        }
                    }
                    if (item.ContentType == null)
                    {
                        item.ContentType = "application/octet-stream";
                    }
                }
            }
            else
            {
                var down = Lib.Helper.DownloadHelper.DownloadFile(item.FullFileUrl);
                if (down != null)
                {
                    item.Binary = down;
                }
            }

            if (item.Binary != null)
            {
                ExternalCacheRender.AddOrUpdate(item);
            }
        }

    }


}
