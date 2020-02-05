using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class PromotionRuleService : ServiceBase<PromotionRule>
    {

        public void CalculatePromotion(RenderContext Context)
        {
            var commerceContext = ServiceProvider.GetCommerceContext(Context); 
         }

    }
}
