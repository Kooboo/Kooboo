using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Response
{
    public class StringResponse : IPaymentResponse
    {
        public string Content { get; set; }
        public Guid requestId { get; set; }
        public string paymemtMethodReferenceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type => EnumResponseType.redirect;
    }
}
