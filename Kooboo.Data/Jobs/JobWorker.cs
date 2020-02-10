//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Timers;

namespace Kooboo.Jobs
{
    public class JobWorker
    {
        public static JobWorker Instance = new JobWorker();

        private bool _running;
        private Timer _timer;

        public JobWorker()
        {
            this._timer = new Timer(10000);
            this._timer.Elapsed += RunJobs;
        }

        public void Start()
        {
            this._timer.Start();
        }

        private void RunJobs(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (_running)
            {
                return;
            }

            _running = true;

            try
            {

                RunSystemWorker();

            }
            catch (Exception)
            {
                // TODO: logging to fallback log store
            } 
            _running = false;
        }


        private bool _SystemworkerRunning;
        private void RunSystemWorker()
        {
            if (_SystemworkerRunning)
            {
                return;
            }
            else
            {
                _SystemworkerRunning = true;
            }

            foreach (var item in JobContainer.BackgroundWorkers)
            {
                TimeSpan span = DateTime.Now - item.LastExecute;
                if (span.TotalSeconds > item.Interval)
                {
                    try
                    {
                        item.Execute();
                    }
                    catch (Exception ex)
                    {
                        Kooboo.Data.Log.Instance.Exception.Write(ex.Message + ex.StackTrace + ex.Source);
                    }
                    item.LastExecute = DateTime.Now;
                }
            }

            _SystemworkerRunning = false;
        }


        public void Stop()
        {
            _timer.Stop();
            _timer = null;
        }
    }
}
