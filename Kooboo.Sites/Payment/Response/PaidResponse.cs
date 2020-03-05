using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Response
{
    public class PaidResponse : IPaymentResponse
    {
        public PaidResponse()
        { 
           
        }
         
        public Guid requestId { get; set; }
        public string paymemtMethodReferenceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type => EnumResponseType.paid; 
    }
}
