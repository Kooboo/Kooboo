using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Web.Payment.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type { get; set; }
    }
}
