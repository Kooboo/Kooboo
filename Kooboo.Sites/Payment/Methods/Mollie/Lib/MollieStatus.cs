namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    // https://docs.mollie.com/payments/status-changes
    public enum MollieStatus
    {
        open,

        canceled,

        pending,

        authorized,

        expired,

        failed,

        paid
    }
}