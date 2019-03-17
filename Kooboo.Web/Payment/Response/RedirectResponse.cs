using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Payment.Models
{
    public class RedirectResponse : IPaymentResponse
    { 
        public RedirectResponse(string redirectUrl, Guid requestId)
        {
            RedirectUrl = redirectUrl; 
        }

        public bool ActionRequired {get;  set;  }

        public string RedirectUrl { get; set; }

        // backward compatible. 
        public string approval_url { get
            {
                return this.RedirectUrl; 
            } }
         
        public Guid PaymentRequestId { get; set; }
        public string PaymentReferenceId { get; set; }
    }
}
