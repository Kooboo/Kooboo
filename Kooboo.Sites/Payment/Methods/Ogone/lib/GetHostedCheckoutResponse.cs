using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class GetHostedCheckoutResponse
    {
        public CreatedPaymentOutput CreatedPaymentOutput { get; set; } = null;

        public string Status { get; set; } = null;
    }

    public class CreatedPaymentOutput
    {
        public Payment Payment { get; set; }

        [ObsoleteAttribute("Use Payment.statusOutput.statusCategory instead")]
        public string PaymentStatusCategory { get; set; } = null;

        public bool? TokenizationSucceeded { get; set; } = null;

        public string Tokens { get; set; } = null;
    }

    public class Payment
    {
        public HostedCheckoutSpecificOutput HostedCheckoutSpecificOutput { get; set; } = null;

        public string Status { get; set; }

        public PaymentStatusOutput StatusOutput { get; set; } = null;
    }

    public class PaymentStatusOutput
    {
        public IList<APIError> Errors { get; set; } = null;

        public bool? IsCancellable { get; set; } = null;

        public string StatusCategory { get; set; } = null;

        public int? StatusCode { get; set; } = null;
    }

    public class APIError
    {
        public string Category { get; set; } = null;

        public string Code { get; set; } = null;

        public int? HttpStatusCode { get; set; } = null;

        public string Id { get; set; } = null;

        public string Message { get; set; } = null;

        public string PropertyName { get; set; } = null;

        public string RequestId { get; set; } = null;
    }

    public class HostedCheckoutSpecificOutput
    {
        public string HostedCheckoutId { get; set; } = null;

        public string Variant { get; set; } = null;
    }
}
