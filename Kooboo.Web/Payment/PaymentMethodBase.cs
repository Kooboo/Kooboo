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

        public abstract IPaymentResponse MakePayment(PaymentRequest request, RenderContext Context);

        public TSetting GetSetting(RenderContext context)
        {
            return PaymentManager.GetPaymentSetting<TSetting>(context);
        }

        public PaymentRequest GetRequest(Guid PaymentRequestId, RenderContext context)
        {
            return PaymentManager.GetRequest(PaymentRequestId, context);
        }

        public void UpdateRequest(PaymentRequest request, RenderContext context)
        {
            PaymentManager.SaveRequest(request, context);
        }

        public string GetCallbackUrl(string MethodName, RenderContext context)
        {
            return PaymentManager.GetCallbackUrl(this, MethodName, context);
        }

        public string EnsureHttpUrl(string AbsOrRelativeUrl, RenderContext context)
        {
            return PaymentManager.EnsureHttp(AbsOrRelativeUrl, context);
        }

        public virtual bool CanUse(RenderContext context)
        {
            var setting = this.GetSetting(context);
            if (setting != null)
            {
                return true;
            }
            return false;
        }
    }
}