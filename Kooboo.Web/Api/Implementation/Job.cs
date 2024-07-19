//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Attributes;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Extensions;
using Kooboo.Data.Language;
using Kooboo.Data.Permission;
using Kooboo.Data.Server;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

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

        [Permission(Feature.JOB, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<SiteJob> List(ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            return siteDb.Job.All().OrderByDescending(it => it.CreationDate);
        }

        [Permission(Feature.JOB, Action = Data.Permission.Action.EDIT)]
        public bool Run(Guid id, ApiCall call)
        {

            var sitedb = call.WebSite.SiteDb();

            var job = sitedb.Job.Get(id);
            if (job == null) throw new Exception("Job not found");
            var jobService = WebHostServer.Services.GetHostedService<JobService>();
            var jobRecord = new JobService.JobRecord(call.WebSite.Id, DateTime.Now, job);
            jobService.ExecuteJob(call.WebSite, jobRecord, $"Manual trigger from {call.Context.User.UserName}");
            return true;
        }

        // [RequireParameters("jobname")]
        // [Permission(Feature.JOB, Action = Data.Permission.Action.VIEW)]
        // public IJob Get(string jobname, ApiCall call)
        // {
        //     var job = Jobs.JobContainer.GetJob(jobname);
        //     job.Context = call.Context;
        //     return job;
        // }

        [Permission(Feature.JOB, Action = Data.Permission.Action.VIEW)]
        public SiteJob GetEdit(ApiCall call, Guid id)
        {
            var model = call.Context.WebSite.SiteDb().Job.Get(id);
            if (model == default) throw new Exception("Job not found");
            return model;
        }


        [RequireParameters("success")]
        [Permission(Feature.JOB, Action = Data.Permission.Action.VIEW)]
        public List<JobLog> Logs(ApiCall call)
        {
            string success = call.GetValue("success");
            bool IsSuccess = false;
            if (!string.IsNullOrEmpty(success) && success?.ToLower() == "true" || success?.ToLower() == "yes")
            {
                IsSuccess = true;
            }

            return GlobalDb.JobLog().GetByWebSiteId(call.Context.WebSite.Id, IsSuccess, 100);
        }

        [Permission(Feature.JOB, Action = Data.Permission.Action.EDIT)]
        public void Post(SiteJob model, ApiCall call)
        {
            var job = call.Context.WebSite.SiteDb().Job.Get(model.Id);
            var jobService = WebHostServer.Services.GetHostedService<JobService>();
            if (job == default)
            {
                job = model;
            }
            else
            {
                job.Repeat = model.Repeat;
                job.Code = model.Code;
                job.Frequence = model.Frequence;
                job.FrequenceUnit = model.FrequenceUnit;
                job.StartTime = model.StartTime;
                job.LastModified = DateTime.Now;
                job.Active = model.Active;
                job.Finish = false;
            }

            call.Context.WebSite.SiteDb().Job.AddOrUpdate(job, call.Context.User.Id);
        }

        [RequireModel(typeof(JobDeleteViewModel))]
        [Permission(Feature.JOB, Action = Data.Permission.Action.DELETE)]
        public void Delete(ApiCall call, Guid id)
        {
            var model = GetEdit(call, id);
            call.Context.WebSite.SiteDb().Job.Delete(model.Id, call.Context.User.Id);
            var jobService = WebHostServer.Services.GetHostedService<JobService>();
        }

        [Permission(Feature.JOB, Action = Data.Permission.Action.DELETE)]
        public void Deletes(ApiCall call, Guid[] ids)
        {
            foreach (var id in ids)
            {
                Delete(call, id);
            }
        }

        [Permission(Feature.JOB, Action = Data.Permission.Action.EDIT)]
        public bool IsUniqueName(ApiCall call)
        {
            return call.WebSite.SiteDb().Job.Get(call.NameOrId) == null;
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
