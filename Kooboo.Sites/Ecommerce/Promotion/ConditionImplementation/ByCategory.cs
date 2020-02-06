using System.Collections.Generic;
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Service;
using Kooboo.Sites.Ecommerce.Models;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.Promotion.ConditionImplementation
{
    /// <summary>
    /// Based on the product category... 
    /// </summary>
    public class ByCategory : IPromotionCondition
    {
        public string Name => "ByProductCategory";

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("ByProductCategory", context); 
        }

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
         

        public bool IsMatch(RenderContext context, PromotionRule rule, Cart cart, CartItem item)
        {
            if (rule.TargetValue == null || !rule.TargetValue.Any())
            {
                return false;
            }
            var productcatservice = ServiceProvider.GetService<ProductCategoryService>(context);

            var allcat = productcatservice.FindCategoies(item.ProductId);

            foreach (var target in rule.TargetValue)
            {
                if (System.Guid.TryParse(target, out System.Guid value))
                {
                    if (allcat.Contains(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public object OptionData(RenderContext context, string Operator)
        {
            var categoryService = ServiceProvider.GetService<CategoryService>(context);
            return categoryService.Tree();
        }
    }
}
