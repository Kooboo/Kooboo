//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Extensions;
using Kooboo.Data.Models;
using Kooboo.IndexedDB.Schedule;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Attributes;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;

namespace Kooboo.Web.Api.Implementation
{
    public class JobApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Job";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public List<JobViewModel> List(ApiCall call)
        {
            var jobs = new List<JobViewModel>();

            foreach (var item in GlobalDb.ScheduleJob().GetByWebSiteId(call.Context.WebSite.Id))
            {
                jobs.Add(new JobViewModel(item));
            }

            foreach (var item in GlobalDb.RepeatingJob().GetByWebSiteId(call.Context.WebSite.Id))
            {
                jobs.Add(new JobViewModel(item));
            }

            SetCodeName(jobs, call.WebSite.SiteDb());

            return jobs;
        }

        private void SetCodeName(List<JobViewModel> jobs, SiteDb sitedb)
        {
            foreach (var item in jobs)
            {
                var code = sitedb.Code.Get(item.CodeId);
                if (code != null)
                {
                    item.CodeName = code.Name;
                }
            }
        }


        public bool Run(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var code = sitedb.Code.Get(id);

            if (code != null)
            {
                Kooboo.Sites.Scripting.Manager.ExecuteCode(call.Context, code.Body, code.Id);
                return true;
            }

            return false;
        }

        [RequireParameters("jobname")]
        public IJob Get(string jobname, ApiCall call)
        {
            var job = Jobs.JobContainer.GetJob(jobname);
            job.Context = call.Context;
            return job;
        }


        public JobViewModel GetEdit(ApiCall call)
        {
            var id = call.GetValue<long>("id");
            if (id > 0)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Job can not edit, you may delete and create a new one"));

                var isrepeat = call.GetBoolValue("IsRepeat");

                if (isrepeat)
                {
                    var item = GlobalDb.RepeatingJob().Get(id);
                    if (item != null)
                    {
                        return new JobViewModel(item);
                    }
                }
                else
                {
                    // var item = GlobalDb.ScheduleJob().
                }
            }

            return new JobViewModel();

        }


        [RequireParameters("success")]
        public List<JobLog> Logs(ApiCall call)
        {
            string success = call.GetValue("success");
            bool IsSuccess = false;
            if (!string.IsNullOrEmpty(success) && success.ToLower() == "true" || success.ToLower() == "yes")
            {
                IsSuccess = true;
            }

            return GlobalDb.JobLog().GetByWebSiteId(call.Context.WebSite.Id, IsSuccess, 100);
        }


        public void Post(JobEditViewModel model, ApiCall call)
        {
            //JobEditViewModel model = call.Context.Request.Model as JobEditViewModel;

            Job newjob = new Job();

            newjob.Description = model.Description;
            newjob.JobName = model.Name;
            newjob.WebSiteId = call.Context.WebSite.Id;
            newjob.Script = model.Script;
            newjob.CodeId = model.CodeId;

            // add a new job. 
            if (model.IsRepeat)
            {
                RepeatItem<Job> repeatjob = new RepeatItem<Job>();
                repeatjob.Item = newjob;
                repeatjob.StartTime = model.StartTime;
                repeatjob.FrequenceUnit = model.FrequenceUnit;

                switch (model.Frequence.ToLower())
                {
                    case "month":
                        {
                            repeatjob.Frequence = RepeatFrequence.Month;
                            break;
                        }
                    case "day":
                        {
                            repeatjob.Frequence = RepeatFrequence.Day;
                            break;
                        }
                    case "minutes":
                        {
                            repeatjob.Frequence = RepeatFrequence.Minutes;
                            break;
                        }
                    case "minute": 
                        {
                            repeatjob.Frequence = RepeatFrequence.Minutes;
                            break;
                        }
                    case "second":
                        {
                            repeatjob.Frequence = RepeatFrequence.Second;
                            break;
                        }
                    case "week":
                        {
                            repeatjob.Frequence = RepeatFrequence.Week;
                            break;
                        }
                    case "hour":
                        {
                            repeatjob.Frequence = RepeatFrequence.Hour;
                            break;
                        }
                    default:
                        break;
                }

                GlobalDb.RepeatingJob().Add(repeatjob); 
                GlobalDb.RepeatingJob().Close();  
            }
            else
            {
                GlobalDb.ScheduleJob().Add(newjob, model.StartTime);
                GlobalDb.ScheduleJob().Close(); 
            } 
        }

        [RequireModel(typeof(JobDeleteViewModel))]
        public void Delete(ApiCall call)
        {
            JobDeleteViewModel model = call.Context.Request.Model as JobDeleteViewModel;

            _delete(model);
        }
        private void _delete(JobDeleteViewModel model)
        {
            if (model.IsRepeat)
            {
                GlobalDb.RepeatingJob().Del(model.Id);
                GlobalDb.RepeatingJob().Close(); 
            }
            else
            {
                GlobalDb.ScheduleJob().Delete(model.DayInt, model.SecondOfDay, model.BlockPosition);
                GlobalDb.ScheduleJob().Close(); 
            }
        }

        public void Deletes(ApiCall call)
        {
 
            try
            {

                string body = call.GetValue("ids");
                if (string.IsNullOrEmpty(body))
                {
                    body = call.Context.Request.Body;
                }

                var   ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(body);

                if (ids !=null)
                {
                    var schedulejobs = GlobalDb.ScheduleJob().GetByWebSiteId(call.Context.WebSite.Id);
                    var repeatjobs = GlobalDb.RepeatingJob().GetByWebSiteId(call.Context.WebSite.Id);

                    foreach (var item in ids)
                    {
                        var findschedule = schedulejobs.Find(o => o.Item.Id == item); 
                        if (findschedule !=null)
                        {
                            GlobalDb.ScheduleJob().Delete(findschedule.DayInt, findschedule.SecondOfDay, findschedule.BlockPosition);
                        }
                        else
                        {
                            var findrepeat = repeatjobs.Find(o => o.Item.Id == item);
                            if (findrepeat !=null)
                            {
                                GlobalDb.RepeatingJob().Del(findrepeat);
                            }

                        }
                    }

                }
            }
            catch (Exception)
            {

            }



        }

    }

    public class FakeJob : IJob
    {
        public RenderContext Context { get; set; }
        public List<JobConfig> Config
        {
            get
            {
                List<JobConfig> result = new List<JobConfig>();
                result.Add(new JobConfig() { Name = Hardcoded.GetValue("sample", Context), ControlType = "input" });
                result.Add(new JobConfig() { Name = Hardcoded.GetValue("doit", Context), ControlType = "CheckBox" });

                return result;
            }
        }

        public Dictionary<string, Type> Configuration
        {
            get
            {
                Dictionary<string, Type> config = new Dictionary<string, Type>();
                config.Add("key", typeof(string));
                return config;
            }
        }

        public bool IsSystemJob
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return "fakename";
            }
        }

        public void Execute(Guid WebSiteId, Dictionary<string, object> Config)
        {
            return;
        }
    }

}
