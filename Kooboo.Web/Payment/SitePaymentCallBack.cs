using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Payment
{
    public class SitePaymentCallBack : IPaymentCallbackWorker
    {
        public void Process(PaymentCallback callback, RenderContext context)
        {
            if (context.WebSite != null && context.WebSite.OrganizationId != default(Guid))
            {
                // TODO: 
                // Execute the ordering fullfill.... 
                // TODO Process local payment.
                if (callback.Paid)
                {
                    // TODO: do the payment ok action... 
                }
                else if (callback.Cancelled)
                {
                    // TODO: do the cancel action. 
                } 
            }
        }
    }
}
