using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.Promotion
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPromotionCondition
     { 
        string Name { get;  }

        string DisplayName(RenderContext context); 

        EnumPromotionTarget TargetObject { get;  }
         
        List<string> AvailableOperators { get; }
 
        /// <summary>
        /// TextBox, Selection, TreeSelection, etc...
        /// </summary>
        string ControlType { get;   }

        /// <summary>
        /// Used for UI target value selection. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        object OptionData(RenderContext context, string Operator);

        // used for text box auto complete.
        List<string> AutoComplete(RenderContext context, string wordpart);

        /// <summary>
        /// Match the condition or not.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cart"></param>
        /// <param name="item">Null if calculating cart discount or shipping cost discount</param>
        /// <returns></returns>
        bool IsMatch (RenderContext context, PromotionRule rule,  Cart cart, CartItem cartitem);
    }
}
