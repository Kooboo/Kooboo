using System;

namespace Kooboo.Mail.Models;

public abstract class MailJob
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public DateTime StartTime { get; set; }

    public bool Active { get; set; }

    public MailMigrationStatus Status { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public Guid OrganizationId { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public void Success()
    {
        Active = false;
        Status = MailMigrationStatus.Success;
        ErrorMessage = string.Empty;
    }

    public void Failure(string error)
    {
        Active = false;
        Status = MailMigrationStatus.Failure;
        ErrorMessage = error;
    }

    public void Running()
    {
        StartTime = DateTime.UtcNow;
        Active = true;
        Status = MailMigrationStatus.Running;
        ErrorMessage = string.Empty;
    }
}

public class MailMigrationJob : MailJob
{
    public const string PasswordKey = "383b2c13-aec3-4993-93a3-301a95a8d60d";

    public string EmailAddress { get; set; }

    public string Host { get; set; }

    public bool ForceSSL { get; set; } = true;

    public int Port { get; set; } = 993;

    public string Password { get; set; }

    public int AddressId { get; set; }
}

public class MailMigrationProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }

    public int FolderId { get; set; }

    public int AddressId { get; set; }

    public int StartIndex { get; set; }

    public int Total { get; set; }

    public int Completed { get; set; }

    public string ErrorMessage { get; set; }

    public MailMigrationStatus Status { get; set; }

    public void Submit(int startIndex)
    {
        StartIndex = startIndex;
        Completed++;
    }

    public void Success()
    {
        Status = MailMigrationStatus.Success;
        ErrorMessage = string.Empty;
    }

    public void Failure(string error)
    {
        ErrorMessage = error;
        Status = MailMigrationStatus.Failure;
    }

    public void Running()
    {
        Status = MailMigrationStatus.Running;
        ErrorMessage = string.Empty;
    }
}

public enum MailMigrationStatus
{
    Unknown = 0,
    Success = 1,
    Failure = 2,
    Running = 3
}