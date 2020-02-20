using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Response
{
    public class RedirectResponse : IPaymentResponse
    {
        public RedirectResponse(string redirectUrl, Guid requestId)
        {
            RedirectUrl = redirectUrl;
        }

        public string RedirectUrl { get; set; }

        // backward compatible. 
        public string approval_url
        {
            get
            {
                return this.RedirectUrl;
            }
        }

        public Guid requestId { get; set; }
        public string paymemtMethodReferenceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type => EnumResponseType.redirect;
    }
}
