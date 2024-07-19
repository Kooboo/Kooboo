using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Server;
using Kooboo.Lib.Helper;
using Kooboo.Lib.Security;
using Kooboo.Mail.Factory;
using Kooboo.Mail.Models;
using Kooboo.Web.Api.Implementation.Mails.ViewModel;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailMigrationApi : IApi
    {
        public string ModelName => "EmailMigration";

        public bool RequireSite => false;

        public bool RequireUser => true;

        [Kooboo.Attributes.RequireModel(typeof(MailMigrationEditViewModel))]
        public void AddJob(ApiCall call)
        {
            var json = call.Context.Request.Body;
            var model = JsonHelper.Deserialize<MailMigrationEditViewModel>(json);
            if (model == null) throw new Exception("model is required");
            var user = call.Context.User;

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = JsonHelper.Deserialize<Dictionary<string, string>>(JsonHelper.Serialize(model));
                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailMigrationApi.AddJob), user, call.Context.Request.Body, dic);
                return;
            }

            var maildb = DBFactory.UserMailDb(user);

            var exists = maildb.MailMigrationJob.Get(model.EmailAddress, model.AddressId);
            if (exists != null)
            {
                throw new Exception("Duplicated job");
            }

            var job = new MailMigrationJob
            {
                Name = model.Name,
                EmailAddress = model.EmailAddress,
                Password = Encryption.Encrypt(model.Password, MailMigrationJob.PasswordKey),
                ForceSSL = model.ForceSSL,
                Port = model.Port,
                Host = model.Host,
                Active = true,
                StartTime = DateTime.UtcNow,
                UserId = user.Id,
                OrganizationId = user.CurrentOrgId,
                AddressId = model.AddressId
            };

            maildb.MailMigrationJob.Add(job);
            var jobService = WebHostServer.Services.GetHostedService<EmailMigrationJob>();
            jobService.EnqueueJob(job);
        }

        [Kooboo.Attributes.RequireModel(typeof(MailMigrationEditViewModel))]
        public void UpdateJob(Guid id, ApiCall call)
        {
            if (id == default)
            {
                throw new Exception("Id is required");
            }

            var json = call.Context.Request.Body;
            var model = JsonHelper.Deserialize<MailMigrationEditViewModel>(json);
            if (model == null) throw new Exception("Model is required");
            var user = call.Context.User;
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = JsonHelper.Deserialize<Dictionary<string, string>>(JsonHelper.Serialize(model));
                dic[nameof(id)] = id.ToString();
                EmailForwardManager.Post<bool>(this.ModelName, nameof(EmailMigrationApi.UpdateJob), user, call.Context.Request.Body, dic);
                return;
            }

            var maildb = DBFactory.UserMailDb(user);

            var job = maildb.MailMigrationJob.Get(id);
            if (job == null || job.Active)
            {
                throw new Exception("Job is null or not active");
            }

            var exists = maildb.MailMigrationJob.Get(model.EmailAddress, model.AddressId);
            if (exists != null && exists.Id != id)
            {
                throw new Exception("Duplicated job");
            }

            var toUpdate = new Dictionary<string, object>
            {
                [nameof(MailMigrationJob.Name)] = model.Name,
                [nameof(MailMigrationJob.EmailAddress)] = model.EmailAddress,
                [nameof(MailMigrationJob.AddressId)] = model.AddressId,
                [nameof(MailMigrationJob.Host)] = model.Host,
                [nameof(MailMigrationJob.ForceSSL)] = model.ForceSSL,
                [nameof(MailMigrationJob.Port)] = model.Port,
            };

            if (!string.IsNullOrEmpty(model.Password))
            {
                toUpdate[nameof(MailMigrationJob.Password)] = Encryption.Encrypt(model.Password, MailMigrationJob.PasswordKey);
            }
            maildb.MailMigrationJob.Update(job, toUpdate);
        }

        public void CancelJob(ApiCall call)
        {
            var user = call.Context.User;
            var id = call.GetValue<string>(nameof(MailMigrationJob.Id));

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>()
                {
                    { "id", id }
                };
                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailMigrationApi.CancelJob), user, dic);
                return;
            }

            var jobService = WebHostServer.Services.GetHostedService<EmailMigrationJob>();
            jobService.CancelJob(user.Id, user.CurrentOrgId, call.ObjectId);
        }

        public bool RunJob(ApiCall call)
        {
            var user = call.Context.User;
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    ["id"] = call.ObjectId.ToString()
                };
                return EmailForwardManager.Post<bool>(this.ModelName, nameof(EmailMigrationApi.RunJob), user, dic);
            }

            var jobService = WebHostServer.Services.GetHostedService<EmailMigrationJob>();
            return jobService.RunJob(user.Id, user.CurrentOrgId, call.ObjectId);
        }

        public void DeleteJob(ApiCall call)
        {
            var user = call.Context.User;
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    ["id"] = call.ObjectId.ToString()
                };
                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailMigrationApi.DeleteJob), user, dic);
                return;
            }

            var jobService = WebHostServer.Services.GetHostedService<EmailMigrationJob>();
            jobService.DeleteJob(user.Id, user.CurrentOrgId, call.ObjectId);
        }

        public IEnumerable<MailMigrationListViewModel> GetJobs(ApiCall call)
        {
            var user = call.Context.User;
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();

                return EmailForwardManager.Get<IEnumerable<MailMigrationListViewModel>>(this.ModelName, nameof(EmailMigrationApi.GetJobs), user, dic);
            }

            var maildb = DBFactory.UserMailDb(user);

            return maildb
                .MailMigrationJob
                .GetAll()
                .Select(item => new MailMigrationListViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    EmailAddress = item.EmailAddress,
                    Host = item.Host,
                    ForceSSL = item.ForceSSL,
                    Port = item.Port,
                    CreationDate = item.CreationDate,
                    LastModified = item.LastModified,
                    Active = item.Active,
                    Status = item.Status.ToString(),
                    AddressId = item.AddressId,
                    ErrorMessage = item.ErrorMessage,
                });
        }
    }
}
