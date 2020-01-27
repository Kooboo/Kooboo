using Kooboo.Web.Payment.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Payment
{
    public interface IPaymentResponse
    {   
        EnumResponseType Type { get; set; }

        bool ActionRequired { get; set; }

        Guid PaymentRequestId { get; set; }
        
        //in case that payment method generate a reference id itself. 
        string PaymemtMethodReferenceId { get; set; }
    }
}
