namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public enum KlarnaStatus
    {
        WAITING = 0,
        BACK = 1,
        IN_PROGRESS = 2,
        COMPLETED = 3,
        CANCELLED = 4,
        FAILED = 5,
        DISABLED = 6,
        ERROR = 7
    }
}