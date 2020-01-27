using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Web.Payment.Response;

namespace Kooboo.Web.Payment.Models
{
    public class PaidResponse : IPaymentResponse
    {
        public PaidResponse()
        {
            this.ActionRequired = false;
            this.Type = EnumResponseType.Paid; 
        }

        public bool ActionRequired { get;  set; }
        public Guid PaymentRequestId { get; set; }
        public string PaymemtMethodReferenceId { get; set; }
        public EnumResponseType Type { get; set; }
    }
}
