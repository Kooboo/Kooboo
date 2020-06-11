//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.SiteTransfer.Download;
using Kooboo.Sites.SiteTransfer.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer
{
    public class TransferManager
    {
        public static TransferTask AddTask(SiteDb siteDb, string StartUrl, int totalcount, int level, Guid UserId = default(Guid))
        {
            TransferTask newtask = new TransferTask();
            newtask.Levels = level;
            newtask.FullStartUrl = StartUrl;
            newtask.TaskType = EnumTransferTaskType.ByLevel;
            newtask.Totalpages = totalcount;
            newtask.UserId = UserId;

            newtask.Domains.Add(UrlHelper.UriHost(StartUrl, true));

            siteDb.TransferTasks.AddOrUpdate(newtask);

            return newtask;
        }

        public static TransferTask AddTask(SiteDb siteDb, List<string> AllUrls, Guid UserId = default(Guid))
        {
            TransferTask newtask = new TransferTask();
            newtask.UserId = UserId;
            newtask.TaskType = EnumTransferTaskType.BySelectedPages;

            bool haslink = false;

            foreach (var item in AllUrls)
            {
                if (!string.IsNullOrEmpty(item) && item.ToLower().StartsWith("http") && Lib.Helper.UrlHelper.IsValidUrl(item, true))
                {
                    TransferPage page = new TransferPage();
                    if (!haslink)
                    {
                        haslink = true;
                        page.DefaultStartPage = true;
                    }
                    page.taskid = newtask.Id;
                    page.absoluteUrl = item;
                    page.done = false;
                    siteDb.TransferPages.AddOrUpdate(page);

                    newtask.Domains.Add(UrlHelper.UriHost(item, true));

                }
            }

            if (!haslink)
            {
                return null;
            }

            siteDb.TransferTasks.AddOrUpdate(newtask);
            return newtask;
        }

        public static TransferTask AddTask(SiteDb siteDb, string Url, string PageRelativeName = null, Guid UserId = default(Guid))
        {
            TransferTask newtask = new TransferTask();
            newtask.TaskType = EnumTransferTaskType.SinglePage;
            newtask.FullStartUrl = Url;
            newtask.UserId = UserId;

            newtask.Domains.Add(UrlHelper.UriHost(Url, true));

            newtask.RelativeName = PageRelativeName;
            siteDb.TransferTasks.AddOrUpdate(newtask);
            return newtask;
        }

        public static async Task ExecuteTask(SiteDb siteDb, TransferTask transferTask)
        {
            ITransferExecutor executor = null;

            if (transferTask.TaskType == EnumTransferTaskType.ByLevel)
            {
                executor = new Executor.TransferByLevelExecutor();

            }
            else if (transferTask.TaskType == EnumTransferTaskType.BySelectedPages)
            {
                executor = new Executor.TransferBySelectedPagesExecutor();
            }
            else if (transferTask.TaskType == EnumTransferTaskType.SinglePage)
            {
                executor = new Executor.TransferSinglePageExecutor();
            }
            executor.SiteDb = siteDb;
            executor.TransferTask = transferTask;
            await executor.Execute();
        }

        public static async Task<SiteObject> continueDownload(SiteDb siteDb, string RelativeUrl)
        {
            if (!siteDb.WebSite.ContinueDownload)
            { return null; }

            string orgimport = null;

            var history = siteDb.TransferTasks.History().ToList();
            if (history.Count() == 0)
            {
                return null;
            }
            else
            {
                orgimport = history.First();
            }

            /// track failed history...
            Guid downloadid = RelativeUrl.ToHashGuid();

            DownloadFailTrack failtrack = siteDb.DownloadFailedLog.Get(downloadid);

            if (failtrack != null)
            {
                if (failtrack.HistoryTime.Where(o => o > DateTime.Now.AddMinutes(-30)).Any())
                {
                    return null;
                }

                if (failtrack.HistoryTime.Count() > 3)
                {
                    return null;
                }
            }
            else
            {
                failtrack = new DownloadFailTrack();
                failtrack.Id = downloadid;
            }

            var oktoDownload = await siteDb.TransferTasks.CanStartDownload(RelativeUrl);

            if (!oktoDownload)
            {
                return null;
            }


            string fullurl = string.Empty;
            DownloadContent download = null;

            if (RelativeUrl.EndsWith("favicon.ico"))
            {
                return null;
            }

            string hostname = TransferHelper.GetPossibleHostName(RelativeUrl);

            if (!string.IsNullOrEmpty(hostname))
            {
                var findurl = history.Find(o => o.ToLower().EndsWith(hostname.ToLower()));

                if (!string.IsNullOrEmpty(findurl))
                {
                    string newrelative = RelativeUrl.Replace(hostname + "/", "");
                    fullurl = UrlHelper.Combine(findurl, newrelative);
                    var cookiecontianer = siteDb.TransferTasks.GetCookieContainerByFullUrl(fullurl);
                    download = await DownloadHelper.DownloadUrlAsync(fullurl, cookiecontianer);
                }
                else
                { 
                    string newrelative = RelativeUrl.Replace(hostname + "/", "");
                    // check whether it is https or not.  
                    // fullurl = UrlHelper.Combine(hostname, newrelative);
                    var protocol = OrgProtocol(orgimport); 
                    fullurl = protocol + hostname + newrelative;
                    var cookiecontianer = siteDb.TransferTasks.GetCookieContainerByFullUrl(fullurl);
                    download = await DownloadHelper.DownloadUrlAsync(fullurl, cookiecontianer);
                }
            }

            if (download == null)
            {
                foreach (var item in history)
                {
                    fullurl = UrlHelper.Combine(item, RelativeUrl);
                    var cookiecontianer = siteDb.TransferTasks.GetCookieContainerByFullUrl(fullurl);
                    download = await DownloadHelper.DownloadUrlAsync(fullurl, cookiecontianer);
                    if (download != null)
                    {
                        break;
                    }
                }
            }

            ///// 301, 302, will be converted to 200 and return back as well. So it is safe to == 200.
            if (download != null && download.StatusCode == 200)
            {
                DownloadManager downloadManager = new DownloadManager() { SiteDb = siteDb };
                SiteObject downloadobject = TransferHelper.AddDownload(downloadManager, download, fullurl, false, true, orgimport);

                if (downloadobject is Page || downloadobject is View)
                {
                    siteDb.TransferPages.AddOrUpdate(new TransferPage() { absoluteUrl = fullurl, PageId = downloadobject.Id });
                }

                siteDb.TransferTasks.ReleaseDownload(RelativeUrl);
                ///for continue download content... 
                Continue.ContinueTask.Convert(siteDb, downloadobject);
                return downloadobject;
            }
            else
            {
                siteDb.TransferTasks.ReleaseDownload(RelativeUrl);
            }

            //download failed. 
            failtrack.HistoryTime.Add(DateTime.Now);
            siteDb.DownloadFailedLog.AddOrUpdate(failtrack);

            return null;
        }

        public static string OrgProtocol(string orgimport)
        {
            if (!string.IsNullOrWhiteSpace(orgimport))

            {
                string lower = orgimport.ToLower();
                if (lower.StartsWith("https://"))
                {
                    return "https://";
                }
            }
            return "http://";
        }

        public static bool IsUrlBanned(string FullUrl)
        {
            var domainresult = Data.Helper.DomainHelper.Parse(FullUrl);

            if (domainresult != null && !string.IsNullOrWhiteSpace(domainresult.SubDomain))
            {
                if (domainresult.SubDomain.Contains("."))
                {
                    return false;
                }

                string sub = domainresult.SubDomain.ToLower();
                if (sub != "www" && sub.Length > 3)
                {
                    return false;
                }
            }

            var rootdomain = Kooboo.Data.Helper.DomainHelper.GetRootDomain(FullUrl);

            var base64string = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rootdomain));

            string url = Data.Helper.AccountUrlHelper.System("IsUrlBanned");

            url = url += "?base64url=" + base64string;

            return Lib.Helper.HttpHelper.Get<bool>(url);
        }

    }
}
