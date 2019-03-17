using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Payment
{
    public interface IPaymentResponse
    {
        // false == pay success. 
        bool ActionRequired { get; set; }

        Guid PaymentRequestId { get; set; }
        
        // incase that payment method generate a reference id itself. 
        string PaymentReferenceId { get; set; }
    }
}
