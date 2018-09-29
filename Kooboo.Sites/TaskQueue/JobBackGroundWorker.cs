using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.IndexedDB.Schedule;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Scripting;
using System;

namespace Kooboo.Sites.TaskQueue
{
    public class JobBackGroundWorkder : IBackgroundWorker
    {
        private void ExecuteScheduleJob(ScheduleItem<Job> jobinfo)
        {
            try
            {
                var website = Kooboo.Data.GlobalDb.WebSites.Get(jobinfo.Item.WebSiteId);

                if (website != null)
                {
                    RenderContext context = new RenderContext();
                    context.WebSite = website;

                    var sitedb = website.SiteDb();

                    var code = sitedb.Code.Get(jobinfo.Item.CodeId);

                    if (code != null)
                    {
                        Manager.ExecuteCode(context, code.Body, code.Id);
                    }
                    else
                    {
                        AddJobLog(jobinfo.Item.JobName, false, DateTime.Now, jobinfo.Item.WebSiteId, "Job code not found");
                    }
                }

                AddJobLog(jobinfo.Item.JobName, true, DateTime.Now, jobinfo.Item.WebSiteId, "");
            }
            catch (Exception ex)
            {
                AddJobLog(jobinfo.Item.JobName, false, DateTime.Now, jobinfo.Item.WebSiteId, ex.Message);
            }
        }

        private void ExecuteRepeatingJob(RepeatItem<Job> repeatingJob)
        {
            if (repeatingJob != null && repeatingJob.Item != null)
            {
                try
                {
                    var website = Kooboo.Data.GlobalDb.WebSites.Get(repeatingJob.Item.WebSiteId);

                    if (website != null)
                    {
                        RenderContext context = new RenderContext();
                        context.WebSite = website;

                        var sitedb = website.SiteDb();

                        var code = sitedb.Code.Get(repeatingJob.Item.CodeId);

                        if (code != null)
                        {
                            Manager.ExecuteCode(context, code.Body, code.Id);
                        }
                        else
                        {
                            AddJobLog(repeatingJob.Item.JobName, false, DateTime.Now, repeatingJob.Item.WebSiteId, "Job code not found");
                        }
                    }
                    else
                    {
                        // not needed, since no website no where to show...
                    }

                    AddJobLog(repeatingJob.Item.JobName, true, DateTime.Now, repeatingJob.Item.WebSiteId, "");

                }
                catch (Exception ex)
                {
                    AddJobLog(repeatingJob.Item.JobName, false, DateTime.Now, repeatingJob.Item.WebSiteId, ex.Message);
                }
            }
        }

        private static void SetOption(Jint.Options option)
        {
            option.MaxStatements(500);
            option.Strict(false);
        }
        
        private static void AddJobLog(string JobName, bool isSuccess, DateTime ExecutionTime, Guid WebSiteId, string message)
        {
            GlobalDb.JobLog().Add(new JobLog()
            {
                JobName = JobName,
                Success = isSuccess,
                ExecutionTime = ExecutionTime,
                WebSiteId = WebSiteId,
                Message = message
            });
        }

        public int Interval => 20;

        public DateTime LastExecute { get; set; }

        public void Execute()
        {

            var scheduleJob = GlobalDb.ScheduleJob().DeQueue();
            while (scheduleJob != null)
            {
                ExecuteScheduleJob(scheduleJob);
                scheduleJob = GlobalDb.ScheduleJob().DeQueue();
            }

            var repeatingJob = GlobalDb.RepeatingJob().DequeueItem();
            while (repeatingJob != null)
            {
                ExecuteRepeatingJob(repeatingJob);
                repeatingJob = GlobalDb.RepeatingJob().DequeueItem();
            }
        }

    }
}


