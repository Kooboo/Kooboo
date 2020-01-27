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
            this.Type = EnumResponseType.paid; 
        }
         
        public Guid requestId { get; set; }
        public string paymemtMethodReferenceId { get; set; }
        public EnumResponseType Type { get; set; }
    }
}
