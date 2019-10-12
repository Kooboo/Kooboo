using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;

namespace Kooboo.Web.Payment
{
    public class SitePaymentCallBack : IPaymentCallbackWorker
    {
        public void Process(PaymentCallback callback, RenderContext context)
        {
            if (context.WebSite != null && context.WebSite.OrganizationId != default(Guid))
            {
                // TODO Process local payment.
                var sitedb = context.WebSite.SiteDb();
                sitedb.GetSiteRepository<SitePaymentCallBackRepository>().AddOrUpdate(callback);
                if (callback.IsPaid)
                {
                    // TODO: do the payment ok action...
                }
                else if (callback.IsCancel)
                {
                    // TODO: do the cancel action.
                }
            }
        }
    }
}