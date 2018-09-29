//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
    public class StyleExecutor : IDownloadExecutor
    {
        public DownloadTask DownloadTask
        { get; set; }

        public DownloadManager Manager
        {
            get; set;
        }

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
           // this.Manager.SiteDb.Routes.EnsureExists(this.DownloadTask.RelativeUrl, ConstObjectType.Style); 
        }

        public void AddDownload(DownloadContent down)
        {
            var sitedb = this.Manager.SiteDb;

            string csstext = down.GetString();
            if (!String.IsNullOrEmpty(csstext))
            {
                Style style = new Style();
                style.Name = UrlHelper.FileName(this.DownloadTask.RelativeUrl);
               CssManager.ProcessResource(ref csstext, this.DownloadTask.AbsoluteUrl, this.Manager, this.DownloadTask.OwnerObjectId);
                style.Body = csstext;
                sitedb.Routes.AddOrUpdate(this.DownloadTask.RelativeUrl, style, this.Manager.UserId);
                sitedb.Styles.AddOrUpdate(style, this.Manager.UserId);
            }
        } 
    }
}
