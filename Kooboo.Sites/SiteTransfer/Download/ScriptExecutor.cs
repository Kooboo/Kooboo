//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
    public class ScriptExecutor : IDownloadExecutor
    {
        public DownloadTask DownloadTask
        { get; set; }

        public DownloadManager Manager
        { get; set; }

        public async Task Execute()
        {
            var download = await DownloadHelper.DownloadUrlAsync(this.DownloadTask.AbsoluteUrl, Manager.CookieContainer);
            if (download != null)
            {
                AddDownload(download);
            }
            else
            {            
                this.Manager.AddRetry(this.DownloadTask);  
            }

            // this.Manager.SiteDb.Routes.EnsureExists(this.DownloadTask.RelativeUrl, ConstObjectType.Script); 
        }

        public void AddDownload(DownloadContent download)
        {
            if (download != null && !string.IsNullOrEmpty(download.GetString()))
            {
                Script script = new Script();
                script.Body = download.GetString();
                script.Name = UrlHelper.FileName(this.DownloadTask.RelativeUrl);
                this.Manager.SiteDb.Routes.AddOrUpdate(this.DownloadTask.RelativeUrl, script, this.Manager.UserId);
                this.Manager.SiteDb.Scripts.AddOrUpdate(script, this.Manager.UserId);
            }
        }

    }
}
