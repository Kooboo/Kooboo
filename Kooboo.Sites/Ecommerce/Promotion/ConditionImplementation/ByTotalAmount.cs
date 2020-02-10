using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.Promotion.ConditionImplementation
{
    public class ByTotalAmount : IPromotionCondition
    {
        public string Name => "ByTotalAmount";

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("ByTotalAmount", context);
        }

        public List<string> AvailableOperators
        {
            get
            {
                List<string> matchCommands = new List<string>();
                matchCommands.Add(">=");
                return matchCommands;
            }
        }

        //Control type...
        public string ControlType => "TextBox";

        public EnumPromotionTarget TargetObject => EnumPromotionTarget.ForShoppingCart;

        public List<string> AutoComplete(RenderContext context, string wordpart)
        {
            return null;
        }

        public bool IsMatch(RenderContext context, PromotionRule rule, Cart cart, CartItem cartitem)
        {
            var carttotal = cart.ItemTotal;

            foreach (var item in rule.TargetValue)
            {
                var value = Kooboo.Lib.Reflection.TypeHelper.ChangeType(item, typeof(decimal));
                if (value != null)
                {
                    var decvalue = (decimal)value;
                    if (decvalue > 0)
                    {
                        if (carttotal >= decvalue)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public object OptionData(RenderContext context, string Operator)
        {
            return null;
        }
    }
}
