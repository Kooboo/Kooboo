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
        public static TransferTask AddTask(SiteDb siteDb, string startUrl, int totalcount, int level, Guid userId = default(Guid))
        {
            TransferTask newtask = new TransferTask
            {
                Levels = level,
                FullStartUrl = startUrl,
                TaskType = EnumTransferTaskType.ByLevel,
                Totalpages = totalcount,
                UserId = userId
            };

            newtask.Domains.Add(UrlHelper.UriHost(startUrl, true));

            siteDb.TransferTasks.AddOrUpdate(newtask);

            return newtask;
        }

        public static TransferTask AddTask(SiteDb siteDb, List<string> allUrls, Guid userId = default(Guid))
        {
            TransferTask newtask = new TransferTask {UserId = userId, TaskType = EnumTransferTaskType.BySelectedPages};

            bool haslink = false;

            foreach (var item in allUrls)
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

        public static TransferTask AddTask(SiteDb siteDb, string url, string pageRelativeName = null, Guid userId = default(Guid))
        {
            TransferTask newtask = new TransferTask
            {
                TaskType = EnumTransferTaskType.SinglePage, FullStartUrl = url, UserId = userId
            };

            newtask.Domains.Add(UrlHelper.UriHost(url, true));

            newtask.RelativeName = pageRelativeName;
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

        public static async Task<SiteObject> continueDownload(SiteDb siteDb, string relativeUrl)
        {
            if (!siteDb.WebSite.ContinueDownload)
            { return null; }

            var history = siteDb.TransferTasks.History().ToList();
            if (history.Count == 0)
            {
                return null;
            }

            // track failed history...
            Guid downloadid = relativeUrl.ToHashGuid();

            DownloadFailTrack failtrack = siteDb.DownloadFailedLog.Get(downloadid);

            if (failtrack != null)
            {
                if (failtrack.HistoryTime.Any(o => o > DateTime.Now.AddMinutes(-30)))
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

            var oktoDownload = await siteDb.TransferTasks.CanStartDownload(relativeUrl);

            if (!oktoDownload)
            {
                return null;
            }

            string fullurl = string.Empty;
            DownloadContent download = null;

            string hostname = TransferHelper.GetPossibleHostName(relativeUrl);

            if (!string.IsNullOrEmpty(hostname))
            {
                var findurl = history.Find(o => o.ToLower().EndsWith(hostname.ToLower()));

                if (!string.IsNullOrEmpty(findurl))
                {
                    string newrelative = relativeUrl.Replace(hostname + "/", "");
                    fullurl = UrlHelper.Combine(findurl, newrelative);
                    var cookiecontianer = siteDb.TransferTasks.GetCookieContainerByFullUrl(fullurl);
                    download = await DownloadHelper.DownloadUrlAsync(fullurl, cookiecontianer);
                }
            }

            if (download == null)
            {
                foreach (var item in history)
                {
                    fullurl = UrlHelper.Combine(item, relativeUrl);
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
                SiteObject downloadobject = TransferHelper.AddDownload(downloadManager, download, fullurl, false, true, fullurl);

                if (downloadobject is Page || downloadobject is View)
                {
                    siteDb.TransferPages.AddOrUpdate(new TransferPage() { absoluteUrl = fullurl, PageId = downloadobject.Id });
                }

                siteDb.TransferTasks.ReleaseDownload(relativeUrl);
                //for continue download content...
                Continue.ContinueTask.Convert(siteDb, downloadobject);
                return downloadobject;
            }
            else
            {
                siteDb.TransferTasks.ReleaseDownload(relativeUrl);
            }

            //download failed.
            failtrack.HistoryTime.Add(DateTime.Now);
            siteDb.DownloadFailedLog.AddOrUpdate(failtrack);

            return null;
        }

        public static bool IsUrlBanned(string fullUrl)
        {
            var domainresult = Data.Helper.DomainHelper.Parse(fullUrl);

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

            var rootdomain = Kooboo.Data.Helper.DomainHelper.GetRootDomain(fullUrl);

            var base64String = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rootdomain));

            string url = Data.Helper.AccountUrlHelper.System("IsUrlBanned");

            url = url += "?base64url=" + base64String;

            return Lib.Helper.HttpHelper.Get<bool>(url);
        }
    }
}