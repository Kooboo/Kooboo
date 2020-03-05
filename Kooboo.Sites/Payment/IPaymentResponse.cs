using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Payment
{
    public interface IPaymentResponse
    {   
        [Description(@"Response type, one of below:
 pending: waiting for next action,
        paid: payment successfully,
        failed: payment failed,
        submitData: require additional information, 
        redirect: redirect user to external url to pay,
        qrCode: pay by scan a QR code,
        HiddenForm = return hidden form value with html to post and redirect user.")]
        [JsonConverter(typeof(StringEnumConverter))]
        EnumResponseType Type { get; }
         
        [Description(@"Kooboo Payment request reference id, used to query payment status at: /_api/payment/checkstatus?id={requestId}")]
        Guid requestId { get; set; }
         
        [Description("Set value in case the Payment Method return an reference id itself")]
        string paymemtMethodReferenceId { get; set; }
    }
}
