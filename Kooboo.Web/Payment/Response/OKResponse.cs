using System;

namespace Kooboo.Web.Payment.Models
{
    public class OKResponse : IPaymentResponse
    {
        public OKResponse()
        {
            this.ActionRequired = false;
        }

        public bool ActionRequired { get; set; }
        public Guid PaymentRequestId { get; set; }
        public string PaymentReferenceId { get; set; }
    }
}