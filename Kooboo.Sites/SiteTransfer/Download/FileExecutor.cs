//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
    public class FileExecutor : IDownloadExecutor
    {
        public DownloadTask DownloadTask
        { get; set; }

        public DownloadManager Manager
        { get; set; }

        public async Task Execute()
        {
            byte[] fileBytes = await DownloadHelper.DownloadFileAsync(this.DownloadTask.AbsoluteUrl, Manager.CookieContainer);
            if (fileBytes != null && fileBytes.Length > 0)
            {
                this.AddDownload(fileBytes);
            }
            else
            {
                this.Manager.AddRetry(this.DownloadTask);
            }

            //this.Manager.SiteDb.Routes.EnsureExists(this.DownloadTask.RelativeUrl, ConstObjectType.File);
        }

        public void AddDownload(byte[] bytes)
        {
            CmsFile kooboofile = new CmsFile();
            kooboofile.Extension = UrlHelper.FileExtension(kooboofile.Name);
            kooboofile.Name = UrlHelper.FileName(this.DownloadTask.RelativeUrl);
            kooboofile.ContentBytes = bytes;
            this.Manager.SiteDb.Routes.AddOrUpdate(this.DownloadTask.RelativeUrl, kooboofile, Manager.UserId);
            this.Manager.SiteDb.Files.AddOrUpdate(kooboofile, this.Manager.UserId);
        }
    }
}