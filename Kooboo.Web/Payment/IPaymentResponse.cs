using System;

namespace Kooboo.Web.Payment
{
    public interface IPaymentResponse
    {
        // false == pay success.
        bool ActionRequired { get; set; }

        Guid PaymentRequestId { get; set; }

        // incase that payment method generate a reference id itself.
        string PaymentReferenceId { get; set; }
    }
}