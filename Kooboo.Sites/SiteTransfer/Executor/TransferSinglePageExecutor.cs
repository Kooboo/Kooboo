//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.SiteTransfer.Download;
using System;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Executor
{
    public class TransferSinglePageExecutor : ITransferExecutor
    {
        public SiteDb SiteDb { get; set; }

        public TransferTask TransferTask { get; set; }

        public async Task Execute()
        {
            if (this.SiteDb == null || this.TransferTask == null || string.IsNullOrEmpty(TransferTask.FullStartUrl))
            {
                return;
            }

            var cookieContainer = new System.Net.CookieContainer();

            TransferPage transpage = new TransferPage
            {
                absoluteUrl = TransferTask.FullStartUrl, taskid = TransferTask.Id
            };

            var oldpage = SiteDb.TransferPages.Get(transpage.Id);

            if (oldpage == null || oldpage.PageId == default(Guid))
            {
                var down = await DownloadHelper.DownloadUrlAsync(transpage.absoluteUrl, cookieContainer);
                if (down == null)
                {
                    return;
                }

                string htmlsource = down.GetString();

                var savepage = new Page();

                string name = TransferTask.RelativeName;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (name.EndsWith("\\"))
                    {
                        name = name.TrimEnd('\\');
                    }

                    if (name.EndsWith("/"))
                    {
                        name = name.TrimEnd('/');
                    }

                    name = System.IO.Path.GetFileNameWithoutExtension(name);
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    savepage.Name = name;
                }

                htmlsource = UrlHelper.ReplaceMetaCharSet(htmlsource);
                string firstImportUrl = SiteDb.TransferTasks.FirstImportHost();

                if (string.IsNullOrEmpty(firstImportUrl))
                {
                    firstImportUrl = TransferTask.FullStartUrl;
                }

                DownloadManager manager = new DownloadManager {SiteDb = SiteDb, OriginalImportUrl = firstImportUrl};

                var context = AnalyzerManager.Execute(htmlsource, transpage.absoluteUrl, savepage.Id, savepage.ConstType, manager, firstImportUrl);

                htmlsource = context.HtmlSource;

                savepage.Body = htmlsource;

                string pageRelativeName = TransferTask.RelativeName;

                if (string.IsNullOrWhiteSpace(pageRelativeName))
                {
                    bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(TransferTask.FullStartUrl, firstImportUrl);
                    pageRelativeName = UrlHelper.RelativePath(TransferTask.FullStartUrl, issamehost);
                }

                if (!pageRelativeName.StartsWith("/"))
                {
                    pageRelativeName = "/" + pageRelativeName;
                }

                SiteDb.Routes.AddOrUpdate(pageRelativeName, savepage.ConstType, savepage.Id, this.TransferTask.UserId);

                SiteDb.Pages.AddOrUpdate(savepage, this.TransferTask.UserId);

                while (!manager.IsComplete)
                {
                    System.Threading.Thread.Sleep(15);
                }

                transpage.done = true;
                transpage.PageId = savepage.Id;
                SiteDb.TransferPages.AddOrUpdate(transpage);
            }

            this.SiteDb.TransferTasks.SetDone(this.TransferTask.Id);
        }
    }
}