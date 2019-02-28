//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Jobs
{
    class ThrottledTaskPool : IDisposable
    {
        private System.Threading.SemaphoreSlim _semaphore;

        public ThrottledTaskPool()
            : this(Environment.ProcessorCount)
        {
        }

        public ThrottledTaskPool(int maxConcurrentTasks)
        {
            _semaphore = new System.Threading.SemaphoreSlim(maxConcurrentTasks, maxConcurrentTasks);
        }

        public void Run(Action action)
        {
            _semaphore.Wait();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    // This task pool is used for jobs only, so the actions are guaranteed to not throw exceptions
                    action();
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
