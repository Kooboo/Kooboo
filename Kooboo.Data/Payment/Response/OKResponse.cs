using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.ServerData.Payment.Models
{
    public class OKResponse : IPaymentResponse
    {
        public OKResponse()
        {
            this.ActionRequired = false; 
        }

        public bool ActionRequired { get;  set; }
        public Guid PaymentRequestId { get; set; }
        public string PaymentReferenceId { get; set; }
    }
}
