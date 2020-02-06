using System.Collections.Generic;
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Service;
using Kooboo.Sites.Ecommerce.Models;

namespace Kooboo.Sites.Ecommerce.Promotion.ConditionImplementation
{
    public class ByCategory : IPromotionCondition
    {
        public string Name => "ByCategory";
        public List<string> AvailableOperators
        {
            get
            {
                List<string> matchCommands = new List<string>();
                matchCommands.Add("==");
                return matchCommands;
            }
        }

        //Control type... 
        public string ControlType => "Tree";

        public EnumPromotionTarget TargetObject => EnumPromotionTarget.ForProduct;

        public List<string> AutoComplete(RenderContext context, string wordpart)
        {
            return null;
        }

        public bool IsMatch(RenderContext context, Cart cart, CartItem item)
        {
            return true;
        }

        public object OptionData(RenderContext context, string Operator)
        {
            var categoryService = ServiceProvider.GetService<CategoryService>(context);
            return categoryService.Tree();
        }
    }
}
