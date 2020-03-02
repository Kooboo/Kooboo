using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
   public class ProductVariantsService : ServiceBase<ProductVariants>
    { 
        public List<ProductVariants> ListByProduct(Guid ProductId)
        {
           return this.Repo.Query.Where(o => o.ProductId == ProductId).SelectAll(); 
        }
    }
}
