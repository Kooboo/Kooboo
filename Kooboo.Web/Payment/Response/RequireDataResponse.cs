using KScript;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Payment.Response
{
    public class RequireDataResponse : IPaymentResponse
    { 
        public Guid requestId { get; set; }
        public string paymemtMethodReferenceId { get; set; } 
         
        [Description("GET or POST")]        
        public string HttpMethod { get; set; } 

        [Description("Form submit action url")]
        public string ActionUrl { get; set; }

        public KDictionary Data { get; set; }
        public EnumResponseType Type { get; set; }
    }
}
