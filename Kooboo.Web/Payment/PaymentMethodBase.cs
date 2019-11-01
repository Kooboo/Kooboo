using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Web.Payment.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.Payment
{
    public abstract class PaymentMethodBase<TSetting> : IPaymentMethod<TSetting> where TSetting : IPaymentSetting
    {
        public abstract string Name { get; }

        public abstract string DisplayName { get; }

        public virtual string Icon { get; }

        public virtual string IconType => "img";

        public virtual List<string> ForCurrency => new List<string>();

        public virtual List<string> ExcludeCurrency => new List<string>();

        public virtual PaymentStatusResponse EnquireStatus(PaymentRequest request, RenderContext context)
        {
            var dbrequest = this.GetRequest(request.Id, context);

            if (dbrequest != null && dbrequest.IsPaid)
            {
                return new PaymentStatusResponse() { HasResult = true, IsPaid = true };
            }
            return new PaymentStatusResponse();
        }

        public abstract IPaymentResponse MakePayment(PaymentRequest request, RenderContext context);

        public TSetting GetSetting(RenderContext context)
        {
            return PaymentManager.GetPaymentSetting<TSetting>(context);
        }

        public PaymentRequest GetRequest(Guid paymentRequestId, RenderContext context)
        {
            return PaymentManager.GetRequest(paymentRequestId, context);
        }

        public void UpdateRequest(PaymentRequest request, RenderContext context)
        {
            PaymentManager.SaveRequest(request, context);
        }

        public string GetCallbackUrl(string methodName, RenderContext context)
        {
            return PaymentManager.GetCallbackUrl(this, methodName, context);
        }

        public string EnsureHttpUrl(string absOrRelativeUrl, RenderContext context)
        {
            return PaymentManager.EnsureHttp(absOrRelativeUrl, context);
        }

        public virtual bool CanUse(RenderContext context)
        {
            var setting = this.GetSetting(context);
            return setting != null;
        }
    }
}