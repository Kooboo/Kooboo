//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
    public class ImageExecutor : IDownloadExecutor
    {
        public DownloadTask DownloadTask
        {
            get; set;
        }

        public DownloadManager Manager
        {
            get; set;
        }

        public async Task Execute()
        {
            byte[] imagebytes = await DownloadHelper.DownloadFileAsync(this.DownloadTask.AbsoluteUrl, Manager.CookieContainer);

            if (imagebytes != null)
            {
                this.AddDownload(imagebytes);
            }
            else
            {
                this.Manager.AddRetry(this.DownloadTask);
            }  
        }

        public void AddDownload(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }
            Image koobooimage = new Image();
            koobooimage.Name = UrlHelper.FileName(this.DownloadTask.RelativeUrl);
            koobooimage.Extension = UrlHelper.FileExtension(koobooimage.Name);

            koobooimage.ContentBytes = bytes;
            this.Manager.SiteDb.Routes.AddOrUpdate(this.DownloadTask.RelativeUrl, koobooimage, this.Manager.UserId);
            this.Manager.SiteDb.Images.AddOrUpdate(koobooimage, this.Manager.UserId);
        }
    }
}
