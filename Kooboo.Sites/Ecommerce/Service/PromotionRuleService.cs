using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Promotion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class PromotionRuleService : ServiceBase<PromotionRule>
    {
        public void CalculatePromotion(RenderContext Context, Cart shoppingCart)
        {
            var commerceContext = ServiceProvider.GetCommerceContext(Context);
        }

        public List<PromotionRule> ActiveRules()
        {

            var allrules = this.Repo.All();

            return allrules.Where(o => IsRuleActive(o)).ToList();

            bool IsRuleActive(PromotionRule rule)
            {
                if (rule.ByDate)
                {
                    return rule.EndDate > System.DateTime.Now;
                }
                else
                {
                    return (rule.IsActive);
                }
            }

        }




    }
}
