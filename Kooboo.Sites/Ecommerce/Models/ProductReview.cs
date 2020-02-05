using Kooboo.Sites.Models;
using System;

namespace Kooboo.Sites.Ecommerce.Models
{
   public class ProductReview : CoreObject
    { 
        public Guid ProductId { get; set; }

        public Guid CustomerId { get; set; }

        public byte Score { get; set; }

        public string Review { get; set; } 
        // TODO: add images. 
    }
}
