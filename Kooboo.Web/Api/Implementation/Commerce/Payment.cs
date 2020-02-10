using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Payment;
using Kooboo.Sites.Payment.Repository; 

namespace Kooboo.Web.Api.Implementation.Commerce
{ 
    public class PaymentApi : IApi
    {
        public string ModelName => "Payment";

        public bool RequireSite => false;

        public bool RequireUser => false;

        public PaymentStatusResponse CheckStatus(ApiCall call)
        {
            var id = call.GetValue("paymentrequestid", "refid", "referenceId", "requestid", "id");

            if (!string.IsNullOrEmpty(id))
            {
                var repo = call.Context.WebSite.SiteDb().GetSiteRepository<PaymentRequestRepository>();

                var request = repo.Get(id);
                if (request != null)
                {
                    return PaymentManager.EnquireStatus(request, call.Context);
                }
            } 
            return new PaymentStatusResponse() { Message = "No result" }; 
        } 
    }


}
