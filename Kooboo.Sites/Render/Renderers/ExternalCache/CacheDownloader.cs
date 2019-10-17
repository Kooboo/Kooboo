using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.Renderers.ExternalCache
{
    public class CacheBinaryDownloader
    {
        public CacheBinaryDownloader(CacheObject item)
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
                    ExternalCacheRender.SaveBinaryDisk(item, down.DataBytes);
                    
                    string contenttype=  down.ContentType;

                    if (contenttype == null)
                    {
                        if (item.name != null)
                        {
                            contenttype = Lib.Helper.IOHelper.MimeType(item.name);
                        }
                    }
                    if (contenttype == null)
                    {
                        contenttype = "application/octet-stream";
                    }

                    ExternalCacheRender.CheckOrUpdateContentType(item, contenttype);
                }
            }
            else
            {
                var DownBinary = Lib.Helper.DownloadHelper.DownloadFile(item.FullFileUrl);
                if (DownBinary != null)
                {
                    ExternalCacheRender.SaveBinaryDisk(item, DownBinary);
                }
            } 
        }

    }
}
