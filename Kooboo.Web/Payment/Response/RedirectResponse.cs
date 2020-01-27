using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Web.Payment.Response;

namespace Kooboo.Web.Payment.Models
{
    public class RedirectResponse : IPaymentResponse
    { 
        public RedirectResponse(string redirectUrl, Guid requestId)
        {
            RedirectUrl = redirectUrl;
            this.Type = EnumResponseType.Redirect; 
        }

        public bool ActionRequired {get;  set;  }

        public string RedirectUrl { get; set; }

        // backward compatible. 
        public string approval_url { get
            {
                return this.RedirectUrl; 
            } }
         
        public Guid PaymentRequestId { get; set; }
        public string PaymemtMethodReferenceId { get; set; }
        public EnumResponseType Type { get; set; }
    }
}
