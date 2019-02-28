//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;
using Kooboo.Lib.Helper;
using Kooboo.Sites.SiteTransfer.Download;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.SiteTransfer.Executor
{
    public class TransferBySelectedPagesExecutor : ITransferExecutor
    {
        public SiteDb SiteDb { get; set; }

        public TransferTask TransferTask { get; set; }

        public async Task Execute()
        {
            if (this.SiteDb == null || this.TransferTask == null)
            {
                return; 
            }

            List<TransferPage> pagelist = SiteDb.TransferPages.TableScan.Where(o => o.taskid == TransferTask.Id && o.done == false).SelectAll();

            DownloadManager manager = new DownloadManager() { SiteDb = SiteDb, UserId = this.TransferTask.UserId };
            List<TransferPage> transpages = new List<TransferPage>();

            string baseurl = null;
            bool defaultstart = true; 

            foreach (var item in pagelist)
            {
                if (baseurl == null)
                {
                    baseurl = item.absoluteUrl; 
                } 
                var down = await DownloadHelper.DownloadUrlAsync(item.absoluteUrl, manager.CookieContainer);
                if (down == null || string.IsNullOrEmpty(down.GetString()))
                {
                    item.done = true;
                    SiteDb.TransferPages.AddOrUpdate(item);
                    continue;
                }
                Page page = null;

                string downloadbody = down.GetString();
                Guid sourcehash = Lib.Security.Hash.ComputeHashGuid(downloadbody);
                item.HtmlSourceHash = sourcehash;

                if (!string.IsNullOrEmpty(downloadbody))
                {
                    var result = SiteDb.TransferPages.Query.Where(o => o.HtmlSourceHash == sourcehash).SelectAll();
                    if (result != null && result.Count > 0)
                    {
                        var transferpage = result[0];
                        TransferHelper.AddPageRoute(SiteDb, transferpage.PageId, item.absoluteUrl, baseurl);
                        item.done = true;
                        item.PageId = transferpage.PageId;
                        SiteDb.TransferPages.AddOrUpdate(item);
                        continue;
                    }
                }

                transpages.Add(item);

                SiteObject downloadobject = TransferHelper.AddDownload(manager, down, item.absoluteUrl, defaultstart, true, baseurl);

                if (downloadobject != null && downloadobject is Page)
                {
                    page = downloadobject as Page;
                }
                if (page != null)
                {
                    item.PageId = page.Id;
                }
                  
                UpdateTransferPage(transpages, manager); 

                if (defaultstart)
                {
                    defaultstart = false; 
                }
               // DownloadOnePage(manager, item);
            }

            while (!manager.IsComplete)
            {
                System.Threading.Thread.Sleep(20);
            }

            this.SiteDb.TransferTasks.SetDone(this.TransferTask.Id); 
        }

        private void UpdateTransferPage(List<TransferPage> transpages, DownloadManager manager)
        {
            var runningobjects = manager.RunningObjectIds();
            List<int> doneindex = new List<int>();

            for (int i = 0; i < transpages.Count; i++)
            {
                var item = transpages[i];
                if (item.PageId != default(Guid))
                {
                    if (!runningobjects.Contains(item.PageId))
                    {
                        doneindex.Add(i);
                    }
                }
            }

            foreach (var item in doneindex.OrderByDescending(o => o))
            {
                var page = transpages[item];
                page.done = true;
                manager.SiteDb.TransferPages.AddOrUpdate(page);
                transpages.RemoveAt(item);
            }

        }
        
  
    }
}
