using System;
using System.Collections.Generic;
using Kooboo.Mail.Models;

namespace Kooboo.Mail.Repositories.Sqlite;

public class MailMigrationProgressRepo
{
    public const string TableName = nameof(MailMigrationProgress);

    private readonly MailDb maildb;

    public MailMigrationProgressRepo(MailDb db)
    {
        maildb = db;
    }

    public IEnumerable<MailMigrationProgress> GetActiveProgressByFolder(int folderId)
    {
        return new SqlWhere<MailMigrationProgress>(maildb.SqliteHelper, TableName)
           .Where(it => it.FolderId == folderId && it.Status == MailMigrationStatus.Running)
           .SelectAll();
    }

    public MailMigrationProgress GetOrAdd(Guid jobId, string folder, int addressId)
    {
        var folderId = Folder.ToId(folder);
        var exists = maildb.SqliteHelper.Get<MailMigrationProgress>(TableName, new Dictionary<string, object>
        {
            [nameof(MailMigrationProgress.JobId)] = jobId,
            [nameof(MailMigrationProgress.FolderId)] = folderId,
            [nameof(MailMigrationProgress.AddressId)] = addressId
        });

        if (exists == null)
        {
            exists = new MailMigrationProgress
            {
                JobId = jobId,
                StartIndex = 0,
                FolderId = folderId,
                AddressId = addressId,
                Status = MailMigrationStatus.Running
            };
            maildb.SqliteHelper.Add(exists, TableName);
        }
        return exists;
    }

    public void Update(MailMigrationProgress progress, Dictionary<string, object> values)
    {
        if (progress.Id == default)
        {
            return;
        }
        var oldJob = maildb.SqliteHelper.Get<MailMigrationProgress>(TableName, nameof(MailMigrationProgress.Id), progress.Id);
        if (oldJob != null)
        {
            maildb.SqliteHelper.Update(TableName, nameof(MailMigrationProgress.Id), progress.Id, values);
        }
    }
}
