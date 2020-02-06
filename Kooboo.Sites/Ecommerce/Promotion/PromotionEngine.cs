using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.Promotion
{
    public static class PromotionEngine
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

        private static IPromotionCondition GetCondition(string key)
        {
            if (_conditions.ContainsKey(key))
            {
                return _conditions[key];
            }
            return null;
        }

        public static void CalculatePromotion(Cart cart, RenderContext context)
        {
            var ruleservice = ServiceProvider.GetService<PromotionRuleService>(context);

            var ActiveRules = ruleservice.ActiveRules();

            CalculateCartItemPromotion(cart, context, ActiveRules);

            CalculateCartPromotion(cart, context, ActiveRules); 

        }

        internal static void CalculateCartPromotion(Cart cart, RenderContext context, List<PromotionRule> ActiveRules)
        { 
                Discount cartDiscount = new Discount();

                foreach (var rule in ActiveRules)
                {
                    var condition = GetCondition(rule.ConditionName);

                    if (condition != null && condition.TargetObject == EnumPromotionTarget.ForShoppingCart)
                    {
                        if (condition.IsMatch(context, rule,  cart, null))
                        {
                            var ruleDisocunt = CalculateCartRuleDiscount(context, rule,  cart);
                            if (ruleDisocunt != null && ruleDisocunt.Total > 0)
                            {
                                cartDiscount.items.Add(ruleDisocunt);
                            }
                        }
                    }
               

                if (cartDiscount.Total > 0)
                {
                    cart.Discount = CombineDiscountRules(cartDiscount);
                }
            }
        }

        internal static void CalculateCartItemPromotion(Cart cart, RenderContext context, List<PromotionRule> ActiveRules)
        {
            // calculate items. 
            foreach (var cartitem in cart.Items)
            {
                Discount itemDiscount = new Discount();

                foreach (var rule in ActiveRules)
                {
                    var condition = GetCondition(rule.ConditionName);

                    if (condition != null && condition.TargetObject == EnumPromotionTarget.ForProduct)
                    {
                        if (condition.IsMatch(context, rule, cart, cartitem))
                        {
                            var ruleDisocunt = CalculateCartItemRuleDiscount(context, rule, cartitem);
                            if (ruleDisocunt != null && ruleDisocunt.Total > 0)
                            {
                                itemDiscount.items.Add(ruleDisocunt);
                            }
                        }
                    }
                }

                if (itemDiscount.Total > 0)
                {
                    cartitem.Discount = CombineDiscountRules(itemDiscount);
                }
            }
        }

        internal static DiscountItem CalculateCartItemRuleDiscount(RenderContext context, PromotionRule rule, CartItem item)
        {
            if (rule == null || (rule.Percent == 1 && rule.Amount == 0))
            {
                return null;
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

            if (offprice <=0)
            {
                return null; 
            }

            DiscountItem result = new DiscountItem();

            var reasonobj = rule.GetValue(context.Culture);
            if (reasonobj != null)
            {
                result.Reason = reasonobj.ToString();
            }

            result.Discount = offprice;

            result.RuleId = rule.Id; 

            result.Quantity = item.Quantity;
            result.CanCombine = rule.CanCombine; 
            return result;
        }
         
        internal static DiscountItem CalculateCartRuleDiscount(RenderContext context, PromotionRule rule, Cart cart)
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
            if (rule.Percent > 0 && rule.Percent <= 1)
            {
                offprice = cart.TotalAmount  * rule.Percent;
            }

            if (rule.Amount > 0)
            {
                offprice = offprice + rule.Amount;
            }

            result.Discount = offprice; 
            result.CanCombine = rule.CanCombine; 
            return result;
        }

        public static Discount CombineDiscountRules(Discount discount)
        {
            var exclusiveItems = discount.items.FindAll(o => o.CanCombine == false);
            if (exclusiveItems != null && exclusiveItems.Any())
            {
                // rule one, remove other. 
                discount.items.RemoveAll(o => o.CanCombine); 
                DiscountItem highestvalue = null;
                foreach (var item in discount.items)
                {
                    if (highestvalue == null)
                    {
                        highestvalue = item;
                    }
                    else
                    {
                        if (highestvalue.Total < item.Total)
                        {
                            highestvalue = item;
                        }
                    }
                }

                discount.items.Clear();
                if(highestvalue !=null)
                {
                    discount.items.Add(highestvalue);
                }
               
            }

            return discount; 
        }

    }
}
