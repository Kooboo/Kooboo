//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using System; 
using Kooboo.IndexedDB.Schedule;
using Kooboo.Data.Models;
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
                //var scheduleJob = GlobalDb.ScheduleJob().DeQueue();
                //while (scheduleJob != null)
                //{
                //    ExecuteScheduleJob(scheduleJob);
                //    scheduleJob = GlobalDb.ScheduleJob().DeQueue();
                //}

                //var repeatingJob = GlobalDb.RepeatingJob().DequeueItem();
                //while (repeatingJob != null)
                //{ 
                //    ExecuteRepeatingJob(repeatingJob); 
                //    repeatingJob = GlobalDb.RepeatingJob().DequeueItem();
                //}

                RunSystemWorker();

            }
            catch (Exception)
            {
                // TODO: logging to fallback log store
            }

            _running = false;
        }

        private void ExecuteScheduleJob(ScheduleItem<Job> jobinfo)
        {
            try
            {
                var job = JobContainer.GetJob(jobinfo.Item.JobName);
                if (job == null)
                {
                    AddJobLog(jobinfo.Item.JobName,null, false, DateTime.Now, jobinfo.Item.WebSiteId, "job not found, name: " + jobinfo.Item.JobName);
                }
                else
                {
                    job.Execute(jobinfo.Item.WebSiteId, jobinfo.Item.Config);
                    AddJobLog(jobinfo.Item.JobName, jobinfo.Item.Description, true, DateTime.Now, jobinfo.Item.WebSiteId, "");
                }
            }
            catch (Exception ex)
            {
                AddJobLog(jobinfo.Item.JobName, jobinfo.Item.Description,  false, DateTime.Now, jobinfo.Item.WebSiteId, ex.Message);
            }
        }

        private void ExecuteRepeatingJob(RepeatItem<Job> repeatingJob)
        {
            if (repeatingJob != null && repeatingJob.Item != null)
            {
                try
                {
                    var job = JobContainer.GetJob(repeatingJob.Item.JobName);
                    if (job == null)
                    {
                        AddJobLog(repeatingJob.Item.JobName, null, false, DateTime.Now, repeatingJob.Item.WebSiteId, "job not found, name: " + repeatingJob.Item.JobName);
                    }
                    else
                    {
                        job.Execute(repeatingJob.Item.WebSiteId, repeatingJob.Item.Config);
                        AddJobLog(repeatingJob.Item.JobName, repeatingJob.Item.Description, true, DateTime.Now, repeatingJob.Item.WebSiteId, "");
                    }
                }
                catch (Exception ex)
                {
                    AddJobLog(repeatingJob.Item.JobName, repeatingJob.Item.Description, false, DateTime.Now, repeatingJob.Item.WebSiteId, ex.Message);
                }
            }
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
                    catch (Exception)
                    {
                        // throw;
                    }
                    item.LastExecute = DateTime.Now; 
                }
            }

            _SystemworkerRunning = false;
        }

        private static void AddJobLog(string JobName, string Description,  bool isSuccess, DateTime ExecutionTime, Guid WebSiteId, string message)
        {
            GlobalDb.JobLog().Add(new JobLog()
            {
                JobName = JobName,
                Description = Description, 
                Success = isSuccess,
                ExecutionTime = ExecutionTime,
                WebSiteId = WebSiteId,
                Message = message
            });
        }

        public void Stop()
        {
            _timer.Stop();
            _timer = null;
        }
    }
}
