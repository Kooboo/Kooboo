//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Kooboo.Mail.Queue
{ 

    public class MailQueueWorker
    {
        public static MailQueueWorker Instance = new MailQueueWorker();
        
        private Timer _timer;

        public MailQueueWorker()
        {
            this._timer = new Timer(120000);
            this._timer.Elapsed += RunJob;
        }

        public void Start()
        {
            this._timer.Start();
        }

        private void RunJob(Object source, System.Timers.ElapsedEventArgs e)
        {
            Queue.QueueManager.Execute().Wait();
        }
         
        public void Stop()
        {
            _timer.Stop();
            _timer = null;
        }
    }
    
}
