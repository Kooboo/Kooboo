namespace Kooboo.Mail
{
    public interface IManagedConnection
    {
        long Id { get; set; }

        void CheckTimeout();
    }
}
