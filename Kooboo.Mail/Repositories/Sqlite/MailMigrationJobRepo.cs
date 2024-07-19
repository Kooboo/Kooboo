using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Models;

namespace Kooboo.Mail.Repositories.Sqlite;

public class MailMigrationJobRepo
{
    private const string TableName = nameof(MailMigrationJob);

    private readonly MailDb maildb;

    public MailMigrationJobRepo(MailDb db)
    {
        maildb = db;
    }

    public List<MailMigrationJob> GetAll()
    {
        return maildb
            .SqliteHelper
            .All<MailMigrationJob>(TableName)
            .OrderByDescending(v => v.CreationDate)
            .ToList();
    }

    public IEnumerable<MailMigrationJob> GetActiveJobsByAddress(IEnumerable<int> addressIds)
    {
        return new SqlWhere<MailMigrationJob>(maildb.SqliteHelper, TableName)
             .Where(it => it.Active)
             .WhereIn(nameof(MailMigrationJob.AddressId), addressIds)
             .SelectAll();
    }

    public void Add(MailMigrationJob job)
    {
        if (job.Id == default)
        {
            job.Id = Guid.NewGuid();
        }

        maildb.SqliteHelper.Add(job, TableName);
    }

    public MailMigrationJob Get(Guid id)
    {
        return maildb.SqliteHelper.Get<MailMigrationJob>(TableName, nameof(MailMigrationJob.Id), id);
    }

    public MailMigrationJob Get(string emailAddress, int addressId)
    {
        return maildb.SqliteHelper.Get<MailMigrationJob>(TableName, new Dictionary<string, object>
        {
            [nameof(MailMigrationJob.EmailAddress)] = emailAddress,
            [nameof(MailMigrationJob.AddressId)] = addressId
        });
    }

    public void Update(MailMigrationJob job, Dictionary<string, object> values)
    {
        if (job.Id == default)
        {
            return;
        }
        var oldJob = maildb.SqliteHelper.Get<MailMigrationJob>(TableName, nameof(MailMigrationJob.Id), job.Id);
        if (oldJob != null)
        {
            maildb.SqliteHelper.Update(TableName, nameof(MailMigrationJob.Id), job.Id, values);
        }
    }

    public void Delete(MailMigrationJob job)
    {
        if (job.Id == default)
        {
            return;
        }

        maildb.SqliteHelper.Delete(MailMigrationProgressRepo.TableName, nameof(MailMigrationProgress.JobId), job.Id);
        maildb.SqliteHelper.Delete(TableName, nameof(MailMigrationJob.Id), job.Id);
    }

    public void DeleteByAddressId(int addressId)
    {
        var jobs = maildb.SqliteHelper.FindAll<MailMigrationJob>(TableName, nameof(MailMigrationJob.AddressId), addressId);
        foreach (var job in jobs)
        {
            Delete(job);
        }
    }
}
