using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.ViewModel;
using Kooboo.Api.ApiResponse;

namespace Kooboo.Web.Api.Implementation
{
    public class PaymentApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "payment"; 
            }
        }

        public bool RequireSite
        {
            get
            {
                return false; 
            }
        }

        public bool RequireUser
        {
            get
            {
                return true; 
            }
        }


        [Attributes.RequireParameters("paymentId", "organizationId")]
        public PaymentStatusResponse PaymentStatus(ApiCall call)
        {
            Guid paymentId = call.GetGuidValue("paymentId");
            Guid organizationId = call.GetGuidValue("organizationId");
            return Data.Service.CommerceService.PaymentStatus(organizationId, paymentId);
        }

        public IResponse  PaypalReturn(ApiCall call)
        {
            var payerId = call.GetValue("payerID");
            var guid = call.GetGuidValue("guid");
            var cancel = call.GetBoolValue("cancel");
            var result= Data.Service.CommerceService.PaypalReturn(payerId, guid,cancel);

            var redirectUrl = System.Net.WebUtility.UrlDecode(call.GetValue("redirectUrl"));
            var response = new MetaResponse();
            response.Success = true;
            response.Redirect(redirectUrl);
            return response;
        }
         
    }
}
