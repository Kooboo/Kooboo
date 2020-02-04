using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Promotion
{
   public interface IPromotionable
    { 
          decimal Discount { get; set; }
          
          List<Guid> DiscountRules { get; set; }

          List<string> DiscountReasons { get; set; }
    } 

}
