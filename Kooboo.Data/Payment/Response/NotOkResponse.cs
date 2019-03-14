using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.ServerData.Payment.Models
{
    public class NotOkResponse : IPaymentResponse
    {
        public NotOkResponse(string error)
        {
            this.Message = error; 
        }

        public bool ActionRequired { get; set; }

        public string Message { get; set; }
        public Guid PaymentRequestId { get; set; }
        public string PaymentReferenceId { get; set; }
    }
}
