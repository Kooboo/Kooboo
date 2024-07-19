namespace Kooboo.Web.Api.Implementation.Mails.ViewModel;

public abstract class MailMigrationBaseViewModel
{
    public string Name { get; set; }

    public string EmailAddress { get; set; }

    public string Host { get; set; }

    public bool ForceSSL { get; set; } = true;

    public int Port { get; set; } = 993;

    public int AddressId { get; set; }
}

public class MailMigrationEditViewModel : MailMigrationBaseViewModel
{
    public string Password { get; set; }
}

public class MailMigrationListViewModel : MailMigrationBaseViewModel
{
    public Guid Id { get; set; }

    public bool Active { get; set; }

    public string Status { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastModified { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}