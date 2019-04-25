//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
    public class DownloadManager
    {
        public DownloadManager()
        {
            this.MaxThread = 30;
            if (Kooboo.Data.AppSettings.IsOnlineServer)
            {
                this.MaxThread = 30;
            }
            else
            {
                this.MaxThread = 500;
            }
            this.CookieContainer = new System.Net.CookieContainer();
        }


        public System.Net.CookieContainer CookieContainer
        {
            get; set;
        }

        public bool EnableDownload { get; set; } = true;

        public HashSet<Guid> AddedAbsUrl = new HashSet<Guid>();

        private List<Guid> pageids = new List<Guid>();

        private int MaxThread { get; set; } = 200;
        private int CurrentThreadCount = 0;

        private bool CanAccept
        {
            get
            { return this.CurrentThreadCount < MaxThread; }
        }

        private object _locker = new object();

        public Queue<DownloadTask> Queue { get; set; } = new Queue<DownloadTask>();

        public SiteDb SiteDb { get; set; }

        // the init transfer url... 
        public string OriginalImportUrl { get; set; }

        public void AddTask(DownloadTask task)
        {
            // lock (_locker)
            //  {
            Guid absId = Lib.Security.Hash.ComputeGuidIgnoreCase(task.AbsoluteUrl);
            if (this.AddedAbsUrl.Contains(absId))
            {
                return;
            }
            else
            {
                this.AddedAbsUrl.Add(absId);
            }

            this.Queue.Enqueue(task);
            /// }

            if (this.EnableDownload)
            {
                StartNewDownload();
            }
        }

        public void AddRetry(DownloadTask task)
        {
            if (task.RetryTimes < 2)
            {
                task.RetryTimes += 1;
                this.Queue.Enqueue(task);
            }
            if (this.EnableDownload)
            {
                StartNewDownload();
            }
        }

        private void StartNewDownload()
        {
            if (this.CanAccept && this.Queue.Count > 0)
            {
                Task.Factory.StartNew(ProcessDownloadsAsync);
            }
        }

        private async void ProcessDownloadsAsync()
        {
            if (this.CanAccept)
            {
                Interlocked.Increment(ref this.CurrentThreadCount);

                try
                {
                    DownloadTask task = null;
                    // lock(_locker)
                    //   {
                    if (this.Queue.Count > 0)
                    {
                        task = this.Queue.Dequeue();
                    }
                    // }
                    if (task != null)
                    {
                        var routeid = Data.IDGenerator.Generate(task.RelativeUrl, ConstObjectType.Route);
                        if (this.SiteDb != null)
                        {
                            var route = this.SiteDb.Routes.Get(routeid);

                            if (route == null || route.objectId == default(Guid))
                            {
                                var exec = GetExecutor(task.ConstType);
                                if (exec != null)
                                {
                                    exec.DownloadTask = task;
                                    await exec.Execute().ConfigureAwait(false);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                }

                Interlocked.Decrement(ref this.CurrentThreadCount);

                if (this.EnableDownload)
                {
                    StartNewDownload();
                }
            }
        }

        private IDownloadExecutor GetExecutor(byte consttype)
        {
            if (consttype == ConstObjectType.Image)
            {
                return new ImageExecutor() { Manager = this };
            }
            else if (consttype == ConstObjectType.CmsFile)
            {
                return new FileExecutor() { Manager = this };
            }
            else if (consttype == ConstObjectType.Style)
            {
                return new StyleExecutor() { Manager = this };
            }
            else if (consttype == ConstObjectType.Script)
            {
                return new ScriptExecutor() { Manager = this };
            }
            return null;
        }

        public HashSet<Guid> RunningObjectIds()
        {
            HashSet<Guid> result = new HashSet<Guid>();
            // lock (_locker)
            //   {
            foreach (var item in this.Queue)
            {
                result.Add(item.OwnerObjectId);
            }
            // }
            return result;
        }

        public bool IsComplete
        {
            get
            {
                return this.CurrentThreadCount == 0 && this.Queue.Count == 0;
            }
        }

        public Guid UserId { get; set; }

    }
}
