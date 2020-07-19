//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using Kooboo.Sites.Repository;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.SiteTransfer.Download;

namespace Kooboo.Sites.SiteTransfer.Executor
{
    public class TransferByLevelExecutor : ITransferExecutor
    {
        public SiteDb SiteDb { get; set; }

        public TransferTask TransferTask { get; set; }

        private HashSet<Guid> DoneUrlHash = new HashSet<Guid>(); 

        public async Task Execute()
        {
            if (TransferTask == null || string.IsNullOrEmpty(TransferTask.FullStartUrl) || SiteDb == null)
            {
                return; 
            }  
            string relativeurl = UrlHelper.RelativePath(TransferTask.FullStartUrl, true);

            TransferProgress progress = new TransferProgress();
            progress.TaskId = TransferTask.Id;
            progress.counter = 0;
            progress.SetDomain(TransferTask.FullStartUrl);
            progress.FullStartUrl = TransferTask.FullStartUrl;
            progress.TotalPages = TransferTask.Totalpages;
            progress.Levels = TransferTask.Levels; 

            TransferPage page = new TransferPage();
            page.absoluteUrl = TransferTask.FullStartUrl;
            page.depth = 0;
            page.taskid = TransferTask.Id;
            page.DefaultStartPage = true;
            page.done = false;

            var oldpage = SiteDb.TransferPages.Get(page.Id);
            if (oldpage == null)
            {
                SiteDb.TransferPages.AddOrUpdate(page);
                progress.counter += 1;
            }

            DownloadManager manager = new DownloadManager() { SiteDb = SiteDb, OriginalImportUrl = TransferTask.FullStartUrl, UserId  = this.TransferTask.UserId }; 

            await  Downloads(SiteDb,  progress, manager);
            
            while(!manager.IsComplete)
            {
                System.Threading.Thread.Sleep(500); 
            } 

            SiteDb.TransferTasks.SetDone(TransferTask.Id); 
        }
          
        private async Task Downloads(SiteDb siteDb, TransferProgress progress, DownloadManager manager)
        {
            List<TransferPage> transferingPages = new List<TransferPage>();

            List<TransferPage> lowerPriorityPages = new List<TransferPage>(); 

            var query = siteDb.TransferPages.Query.Where(o => o.taskid == progress.TaskId && o.done == false);

            while (true)
            {
                List<TransferPage> pagelist = query.SelectAll();
                pagelist.RemoveAll(o => DoneUrlHash.Contains(o.Id)); 
                if (pagelist == null || pagelist.Count == 0)
                {
                    if (progress.counter < progress.TotalPages && lowerPriorityPages.Count()>0)
                    {
                        var needed = progress.TotalPages - progress.counter;
                        var neededpages = lowerPriorityPages.Take(needed);

                        foreach (var item in neededpages)
                        {
                            progress.counter += 1;
                            siteDb.TransferPages.AddOrUpdate(item);
                            lowerPriorityPages.Remove(item); 
                        } 
                        continue;  
                    }
                    else
                    {
                        break;
                    } 
                }

                foreach (var item in pagelist)
                {
                    DoneUrlHash.Add(item.Id); 

                    var down =  await DownloadHelper.DownloadUrlAsync(item.absoluteUrl, manager.CookieContainer);

                    siteDb.TransferTasks.UpdateCookie(progress.TaskId, manager.CookieContainer); 

                    if (down == null || string.IsNullOrEmpty(down.GetString()))
                    {
                        item.done = true;
                        siteDb.TransferPages.AddOrUpdate(item);
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
                            TransferHelper.AddPageRoute(SiteDb, transferpage.PageId, item.absoluteUrl, progress.BaseUrl);
                            item.done = true;
                            item.PageId = transferpage.PageId;
                            SiteDb.TransferPages.AddOrUpdate(item); 
                            continue;
                        }
                    }

                    transferingPages.Add(item);

                    SiteObject downloadobject = TransferHelper.AddDownload(manager, down, item.absoluteUrl, item.DefaultStartPage, true, progress.BaseUrl);

                    if (downloadobject != null && downloadobject is Page)
                    {
                        page = downloadobject as Page;
                    }
                    if (page != null)
                    {
                        item.PageId = page.Id; 
                    }
                       
                    if (page == null || page.Dom == null)
                    {
                        item.done = true;
                        manager.SiteDb.TransferPages.AddOrUpdate(item);
                        continue;
                    }
                      

                    if (progress.counter < progress.TotalPages && item.depth < progress.Levels)
                    {
                        page.Dom.URL = item.absoluteUrl;

                        var links =  TransferHelper.GetAbsoluteLinks(page.Dom, page.Dom.getBaseUrl());

                        foreach (var linkitem in links)
                        {
                            if (progress.counter >= progress.TotalPages)
                            {
                                break;
                            }

                            if (!UrlHelper.isSameHost(linkitem, progress.BaseUrl))
                            {
                                continue;
                            }

                            if (!TransferHelper.IsPageUrl(linkitem))
                            {
                                continue;
                            }

                            TransferPage newpage = new TransferPage();
                            newpage.absoluteUrl = linkitem;
                            newpage.depth = item.depth + 1;
                            newpage.taskid = progress.TaskId; 

                            if (!IsDuplicate(siteDb, newpage))
                            { 
                                if(TransferHelper.IsLowerPrioUrl(linkitem))
                                {
                                    if (lowerPriorityPages.Find(o=>o.Id == newpage.Id) == null)
                                    {
                                        lowerPriorityPages.Add(newpage);
                                    } 
                                }
                                else
                                {
                                    progress.counter += 1;
                                    siteDb.TransferPages.AddOrUpdate(newpage);
                                } 
                            } 
                        }

                    }

                    UpdateTransferPage(transferingPages, manager); 
                }
            }

            while(transferingPages.Count()>0)
            {
                System.Threading.Thread.Sleep(1000);
                UpdateTransferPage(transferingPages, manager); 
            }  
        } 

        private void UpdateTransferPage(List<TransferPage> transpages, DownloadManager manager)
        {
            var runningobjects = manager.RunningObjectIds(); 
            List<int> doneindex = new List<int>();

            for (int i = 0; i < transpages.Count; i++)
            {
                var item = transpages[i]; 
                if (item.PageId !=default(Guid))
                {
                    if (!runningobjects.Contains(item.PageId))
                    {
                        doneindex.Add(i); 
                    }
                }
            }

            foreach (var item in doneindex.OrderByDescending(o=>o))
            {
                var page = transpages[item];
                page.done = true;
                manager.SiteDb.TransferPages.AddOrUpdate(page);
                transpages.RemoveAt(item); 
            }

            //if (runningobjects.Count()==0)
            //{
            //    System.Threading.Thread.Sleep(3000); 
            //    if (runningobjects.Count()==0)
            //    {
            //        transpages.RemoveAll(o => true);
            //    } 
            //} 
        }

        /// <summary>
        /// Check to make sure that this is not a duplicate page. 
        /// </summary>
        /// <param name="newpage"></param>
        /// <returns></returns>
        private bool IsDuplicate(SiteDb sitedb, TransferPage newpage)
        { 
            var oldpage = sitedb.TransferPages.Get(newpage.Id);

            if (oldpage != null)
            {
                return true;
            }

            return false;
        }
          
    }


}