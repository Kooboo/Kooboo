using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Response
{
    public class FailedResponse : IPaymentResponse
    {
        public FailedResponse(string error)
        {
            this.Message = error; 
        } 
        public string Message { get; set; }
        public Guid requestId { get; set; }
        public string paymemtMethodReferenceId { get; set; }
         
        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type  => EnumResponseType.failed;
    }
}
