using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Web.Payment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Payment
{
    public abstract class PaymentMethodBase<TSetting> : IPaymentMethod<TSetting> where TSetting : IPaymentSetting
    { 
        public RenderContext Context { get; set; }
         
        public TSetting Setting { get; set; }

        public RenderContext CurrentContext { get; set; }

        public abstract string Name { get; }

        public abstract string DisplayName { get; }

        public virtual string Icon { get; }

        [Description("Img/Base64")]
        public virtual string IconType => "img";

        public virtual List<string> ForCurrency => new List<string>();
         
        public virtual List<string> ExcludeCurrency => new List<string>();

        public virtual PaymentStatusResponse EnquireStatus(PaymentRequest request)
        {
            var dbrequest = this.GetRequest(request.Id, CurrentContext); 
            
            if (dbrequest !=null && dbrequest.IsPaid)
            {
                return new PaymentStatusResponse() { HasResult = true, Paid = true };
            }  
            return new PaymentStatusResponse();  
        }

        public abstract IPaymentResponse Charge(PaymentRequest request);

        public TSetting GetSetting(RenderContext context)
        {
            return PaymentManager.GetPaymentSetting<TSetting>(context);
        } 

        public PaymentRequest GetRequest(Guid PaymentRequestId, RenderContext context)
        {
            return PaymentManager.GetRequest(PaymentRequestId, context); 
        } 

        public string EnsureHttpUrl(string AbsOrRelativeUrl, RenderContext context)
        {
           return PaymentManager.EnsureHttp(AbsOrRelativeUrl, context);
        }

         
    }
}
