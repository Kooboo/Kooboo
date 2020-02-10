using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Promotion
{
  public   class PromotionResult
    { 
        decimal Discount { get; set; }

        List<Guid> DiscountRules { get; set; }

        List<string> DiscountReasons { get; set; } 

    }
}
