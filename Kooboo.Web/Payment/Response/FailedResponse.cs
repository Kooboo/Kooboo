using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Web.Payment.Response;

namespace Kooboo.Web.Payment.Models
{
    public class FailedResponse : IPaymentResponse
    {
        public FailedResponse(string error)
        {
            this.Message = error;
            this.Type = EnumResponseType.failed;
        } 
        public string Message { get; set; }
        public Guid requestId { get; set; }
        public string paymemtMethodReferenceId { get; set; }
        public EnumResponseType Type { get; set; }
    }
}
