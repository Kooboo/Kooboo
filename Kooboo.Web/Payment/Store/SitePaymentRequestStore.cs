using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using System;

namespace Kooboo.Web.Payment
{
    public class SitePaymentRequestStore : IPaymentRequestStore
    {
        public PaymentRequest Get(Guid PaymentRequestId, RenderContext context)
        {
            if (context.WebSite != null && context.WebSite.OrganizationId != default(Guid))
            {
                var sitedb = context.WebSite.SiteDb();
                if (sitedb != null)
                {
                    var repo = sitedb.GetSiteRepository<SitePaymentRequestRepository>();
                    return repo.Get(PaymentRequestId);
                }
            }
            return null;
        }

        public void Save(PaymentRequest request, RenderContext context)
        {
            if (context.WebSite != null && context.WebSite.OrganizationId != default(Guid))
            {
                var sitedb = context.WebSite.SiteDb();
                if (sitedb != null)
                {
                    var repo = sitedb.GetSiteRepository<SitePaymentRequestRepository>();
                    repo.AddOrUpdate(request);
                }
            }
        }
    }
}