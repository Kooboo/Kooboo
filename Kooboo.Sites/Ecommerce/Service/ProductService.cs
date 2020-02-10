using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class ProductService : ServiceBase<Product>
    { 
        public List<Product> ListByCategory(Guid CategoryId, int skip, int count)
        {
            //TODO: sort product here...
            var list = ServiceProvider.ProductCategory(this.Context).ProductIdList(CategoryId); 
            return this.Repo.Query.WhereIn<Guid>(o => o.Id, list).Skip(skip).Take(count);
        }
    }
}
