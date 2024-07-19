using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Extensions;
using Kooboo.Lib.Security;
using Kooboo.Mail;
using Kooboo.Mail.Models;
using Kooboo.Mail.Utility;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimeKit;

namespace Kooboo.Web.Api.Implementation.Mails;

public class EmailMigrationJob : BackgroundService
{
    private readonly ConcurrentDictionary<string, MailMigrationJob> _jobs = new();

    private readonly ConcurrentDictionary<string, bool> _runningJobs = new();

    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokenSource = new();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
        {
            if (!AppSettings.DisableJob)
            {
                var databases = Directory.GetFiles(AppSettings.DatabasePath, "mail.db", SearchOption.AllDirectories);
                foreach (var database in databases)
                {
                    var helper = new SqlLiteHelper(database);
                    MailDb.InitMailDb(helper);

                    var jobs = helper.All<MailMigrationJob>(nameof(MailMigrationJob))
                        .Where(job => job.Active)
                        .ToList();
                    if (!jobs.Any())
                    {
                        continue;
                    }

                    foreach (var job in jobs)
                    {
                        EnqueueJob(job);
                    }
                }

                _ = ScheduleAsync(stoppingToken);
            }
        });
        return Task.CompletedTask;
    }

    public void EnqueueJob(MailMigrationJob job)
    {
        var key = GetKey(job.UserId, job.OrganizationId, job.Id);
        _jobs[key] = job;
        _cancellationTokenSource[key] = new CancellationTokenSource();
    }

    public bool RunJob(Guid userId, Guid organizationId, Guid jobId)
    {
        var mailDb = new MailDb(userId, organizationId);
        var job = mailDb.MailMigrationJob.Get(jobId);
        if (job == null || job.Active)
        {
            return false;
        }

        job.Running();
        UpdateRecordStatus(mailDb, job);
        EnqueueJob(job);

        return true;
    }

    public bool CancelJob(Guid userId, Guid organizationId, Guid jobId)
    {
        var key = GetKey(userId, organizationId, jobId);

        if (!_jobs.TryGetValue(key, out var job) || !job.Active)
        {
            return false;
        }

        if (_jobs.TryRemove(key, out var _))
        {
            var mailDb = new MailDb(job.UserId, job.OrganizationId);
            job.Failure("The operation was canceled.");
            UpdateRecordStatus(mailDb, job);

            if (_cancellationTokenSource.TryRemove(key, out var cancellationToken))
            {
                cancellationToken.Cancel();
            }
        }

        return true;
    }

    public bool DeleteJob(Guid userId, Guid organizationId, Guid jobId)
    {
        var mailDb = new MailDb(userId, organizationId);
        var job = mailDb.MailMigrationJob.Get(jobId);
        if (job == null || job.Active)
        {
            return false;
        }
        mailDb.MailMigrationJob.Delete(job);
        return true;
    }

    static string GetKey(Guid userId, Guid organizationId, Guid jobId)
    {
        return $"{userId}_{organizationId}_{jobId}";
    }

    private async Task ScheduleAsync(CancellationToken token)
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        while (await periodicTimer.WaitForNextTickAsync(token))
        {
            Parallel.ForEach(_jobs, async item =>
            {
                if (!item.Value.Active || _runningJobs.ContainsKey(item.Key)) return;

                _runningJobs[item.Key] = true;
                if (!_cancellationTokenSource.TryGetValue(item.Key, out var cancellationToken))
                {
                    cancellationToken = new CancellationTokenSource();
                }
                await CheckEmails(item.Value, cancellationToken.Token);
                _runningJobs.TryRemove(item.Key, out var _);
            });
        }
    }

    private static void EnsureClientConnected(ImapClient client, MailMigrationJob job, CancellationToken token)
    {
        if (client.IsConnected)
        {
            return;
        }

        client.Connect(job.Host, job.Port, job.ForceSSL, token);
        var password = job.Password;
        try
        {
            password = Encryption.Decrypt(job.Password, MailMigrationJob.PasswordKey);
        }
        catch (CryptographicException)
        {
            job.Password = Encryption.Encrypt(job.Password, MailMigrationJob.PasswordKey);
            var mailDb = new MailDb(job.UserId, job.OrganizationId);
            UpdateRecordStatus(mailDb, job);
        }
        client.Authenticate(job.EmailAddress, password, token);

        if (client.Capabilities.HasFlag(ImapCapabilities.Id))
        {
            client.Identify(new ImapImplementation
            {
                Name = "Kooboo mail service",
                Version = "1.0.0"
            }, token);
        }
    }

    private static async Task CheckEmails(MailMigrationJob job, CancellationToken token)
    {
        var mailDb = new MailDb(job.UserId, job.OrganizationId);
        using var client = new ImapClient();

        try
        {
            EnsureClientConnected(client, job, token);

            job.Running();
            UpdateRecordStatus(mailDb, job);

            var folders = await client.GetFoldersAsync(new FolderNamespace('/', ""), cancellationToken: token);
            foreach (var folder in folders)
            {
                EnsureClientConnected(client, job, token);
                await GetMailsByFolder(mailDb, folder, job, token);
            }

            job.Success();
            UpdateRecordStatus(mailDb, job);
        }
        catch (Exception ex)
        {
            job.Failure(ex.Message);
            UpdateRecordStatus(mailDb, job);
        }
        finally
        {
            await client.DisconnectAsync(true, token);
        }
    }

    private static async Task GetMailsByFolder(MailDb mailDb, IMailFolder folder, MailMigrationJob job, CancellationToken token)
    {
        await folder.OpenAsync(FolderAccess.ReadOnly, token);
        if (SkipFolder(folder))
        {
            await folder.CloseAsync(false, token);
            return;
        }
        if (folder.UidNext == null && folder.Count == 0 && (folder.Attributes & FolderAttributes.NoSelect) != 0)
        {
            await folder.CloseAsync(false, token);
            return;
        }

        var folderName = GetFolderName(folder);

        var kbFolder = Folder.ReservedFolder.ContainsValue(folderName) ? new Folder(folderName) : mailDb.Folder.Get(folderName, isMigration: true);
        if (kbFolder == null)
        {
            kbFolder = new Folder(folderName);
            mailDb.Folder.Add(folderName);
        }

        var progress = mailDb.MailMigrationProgress.GetOrAdd(job.Id, folderName, job.AddressId);
        if (progress.StartIndex < 0)
        {
            progress.StartIndex = 0;
        }

        try
        {
            var ustart = new UniqueId(Convert.ToUInt32(progress.StartIndex + 1));
            var uend = (folder.UidNext == null ? UniqueId.MaxValue : folder.UidNext).Value;
            var range = new UniqueIdRange(ustart, uend);

            var uids = await folder.SearchAsync(SearchQuery.Uids(range), token);
            if (uids.Count <= 0)
            {
                progress.Success();
                UpdateJobProgress(mailDb, progress);

                await folder.CloseAsync(false, token);
                return;
            }

            progress.Total = uids.Count;
            progress.Completed = 0;
            progress.Running();
            UpdateJobProgress(mailDb, progress);

            foreach (var uid in uids.OrderBy(it => it.Id))
            {
                //using var stream = await folder.GetStreamAsync(uid, 0, int.MaxValue, token);
                var msg = await folder.GetMessageAsync(uid, token);
                if (msg == null)
                {
                    continue;
                }

                var message = MessageUtility.ParseMeta(msg);

                if (string.IsNullOrEmpty(message.SmtpMessageId))
                {
                    message.SmtpMessageId = msg.ToString().ToHashGuid().ToString();
                }
                message.SmtpMessageId = $"{message.SmtpMessageId}_{uid.Id}";
                var oldMsg = mailDb.Message2.Get(message.SmtpMessageId, job.UserId, job.AddressId, kbFolder.Id);
                if (oldMsg != null)
                {
                    progress.Submit((int)uid.Id);
                    UpdateJobProgress(mailDb, progress);
                    continue;
                }
                message.Body = string.IsNullOrEmpty(msg.HtmlBody) ? msg.TextBody : msg.HtmlBody;
                message.FolderId = progress.FolderId;
                message.AddressId = job.AddressId;
                message.UserId = job.UserId;
                message.Recent = false; // 不让前端提醒有新邮件

                var success = mailDb.Message2.AddOrUpdate(message, msg);
                msg.Dispose();
                if (success)
                {
                    progress.Submit((int)uid.Id);
                    UpdateJobProgress(mailDb, progress);
                }

                if ((folder.Attributes & FolderAttributes.Sent) != 0)
                {
                    var addresses = new InternetAddressList();
                    var to = msg.To;
                    if (to is not null)
                        addresses.AddRange(to);
                    var cc = msg.Cc;
                    if (cc is not null)
                        addresses.AddRange(cc);
                    var bcc = msg.Bcc;
                    if (bcc is not null)
                        addresses.AddRange(bcc);
                    if (!addresses.Any())
                    {
                        continue;
                    }
                    AddContactByMailBody(mailDb, job, addresses);
                }
            }

            progress.Success();
            UpdateJobProgress(mailDb, progress);
        }
        catch (Exception folderEx)
        {
            job.Failure(folderEx.Message);
            UpdateRecordStatus(mailDb, job);

            progress.Failure(folderEx.Message);
            UpdateJobProgress(mailDb, progress);
        }


        await folder.CloseAsync(false, token);
    }

    private static void AddContactByMailBody(MailDb mailDb, MailMigrationJob job, InternetAddressList internetAddress)
    {
        foreach (var item in internetAddress)
        {
            // add to contact
            mailDb.AddBook.Add(item.ToString());
        }
    }

    private static bool SkipFolder(IMailFolder folder)
    {
        var skips = new[] { FolderAttributes.All, FolderAttributes.Archive, FolderAttributes.Drafts, FolderAttributes.Flagged, FolderAttributes.Important, FolderAttributes.Junk, FolderAttributes.Trash };

        foreach (var skip in skips)
        {
            if ((folder.Attributes & skip) != 0)
            {
                return true;
            }

            if (CheckParentFolder(folder, skip))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CheckParentFolder(IMailFolder folder, FolderAttributes skip)
    {
        var parentFolder = folder.ParentFolder;
        while (parentFolder != null)
        {
            if ((parentFolder.Attributes & skip) != 0)
            {
                return true;
            }

            parentFolder = parentFolder.ParentFolder;
        }

        return false;
    }

    private static string GetFolderName(IMailFolder folder)
    {
        if (Folder.Inbox.Equals(folder.FullName, StringComparison.OrdinalIgnoreCase))
        {
            return Folder.Inbox;
        }

        if ((folder.Attributes & FolderAttributes.Drafts) != 0)
            return Folder.Drafts;
        if ((folder.Attributes & FolderAttributes.Junk) != 0)
            return Folder.Spam;
        if ((folder.Attributes & FolderAttributes.Sent) != 0)
            return Folder.Sent;
        if ((folder.Attributes & FolderAttributes.Trash) != 0)
            return Folder.Trash;
        return folder.FullName;
    }

    private static void UpdateRecordStatus(MailDb mailDb, MailMigrationJob item)
    {
        mailDb.MailMigrationJob.Update(item, new Dictionary<string, object>
        {
            [nameof(MailMigrationJob.Status)] = (int)item.Status,
            [nameof(MailMigrationJob.Active)] = item.Active,
            [nameof(MailMigrationJob.ErrorMessage)] = item.ErrorMessage,
            [nameof(MailMigrationJob.Password)] = item.Password,
            [nameof(MailMigrationJob.StartTime)] = item.StartTime,
            [nameof(MailMigrationJob.LastModified)] = DateTime.UtcNow,
        });
    }

    private static void UpdateJobProgress(MailDb mailDb, MailMigrationProgress item)
    {
        mailDb.MailMigrationProgress.Update(item, new Dictionary<string, object>
        {
            [nameof(MailMigrationProgress.AddressId)] = item.AddressId,
            [nameof(MailMigrationProgress.StartIndex)] = item.StartIndex,
            [nameof(MailMigrationProgress.Status)] = (int)item.Status,
            [nameof(MailMigrationProgress.Total)] = item.Total,
            [nameof(MailMigrationProgress.Completed)] = item.Completed,
            [nameof(MailMigrationProgress.ErrorMessage)] = item.ErrorMessage,
        });
    }

    class StoreSyncServiceDescriptor : ServiceDescriptor
    {
        public StoreSyncServiceDescriptor() : base(typeof(IHostedService), typeof(EmailMigrationJob), ServiceLifetime.Singleton)
        {
        }
    }
}
