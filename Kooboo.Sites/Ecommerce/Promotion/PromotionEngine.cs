using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.Promotion
{
    public class PromotionEngine
    {

        private static Dictionary<string, IPromotionCondition> _conditions { get; set; }

        static PromotionEngine()
        {
            _conditions = new Dictionary<string, IPromotionCondition>();

            var list = Kooboo.Lib.IOC.Service.GetInstances<IPromotionCondition>();
            foreach (var item in list)
            {
                _conditions[item.Name] = item;
            }
        }

        private IPromotionCondition GetCondition(string key)
        {
            if (_conditions.ContainsKey(key))
            {
                return _conditions[key];
            }
            return null;
        }

        public void CalculateCatPromotion(Cart cart, RenderContext context)
        {
            var ruleservice = ServiceProvider.GetService<PromotionRuleService>(context);

            var ActiveRules = ruleservice.ActiveRules();

            foreach (var cartitem in cart.Items)
            {
                Discount itemDiscount = new Discount();

                foreach (var rule in ActiveRules)
                {
                    var condition = GetCondition(rule.ConditionName);

                    if (condition != null && condition.TargetObject == EnumPromotionTarget.ForProduct)
                    {
                        if (condition.IsMatch(context, cart, cartitem))
                        { 
                            var ruleDisocunt = CalculateCartItemDiscount(context, rule, cartitem); 
                            if (ruleDisocunt !=null && ruleDisocunt.Total >0)
                            {
                                itemDiscount.items.Add(ruleDisocunt); 
                            }
                        }
                    }
                }
            }

            List<PromotionResult> itemresult = new List<PromotionResult>();


            foreach (var item in ruleservice.ActiveRules())
            {
                var condition = GetCondition(item.Name);
                if (condition != null && condition.IsMatch(condition))
            }
        }


        public DiscountItem CalculateCartItemDiscount(RenderContext context, PromotionRule rule, CartItem item)
        {
            if (rule == null || (rule.Percent == 1 && rule.Amount == 0))
            {
                return null;
            }

            DiscountItem result = new DiscountItem();

            var reasonobj = rule.GetValue(context.Culture);
            if (reasonobj != null)
            {
                result.Reason = reasonobj.ToString();
            }

            decimal offprice = 0;
            if (rule.Percent > 0 && rule.Percent < 1)
            {
                offprice = item.UnitPrice * rule.Percent;
            }

            if (rule.Amount > 0)
            {
                offprice = offprice + rule.Amount;
            }

            result.Discount = offprice;

            result.Quantity = item.Quantity;
            result.CanCombine = rule.CanCombine; 

            return result;
        }

        public void CombineDiscountRules(Discount discount)
        {
            var exclusiveItems = discount.items.FindAll(o => o.CanCombine == false); 
            if (exclusiveItems !=null && exclusiveItems.Any())
            {
                // rule one, remove other. 

            }
            else
            {
                return; 
            }
        }

    }
}
