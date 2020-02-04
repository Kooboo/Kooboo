using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Promotion
{
   public class PromotionAction
    {
        public string Name { get; set; }
         
        public decimal Amount { get; set; } = 0;

        public decimal Percent { get; set; } = 1;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        
    }
}
