using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class ProductService : ServiceBase<Product>
    { 
        public List<Product> ByCategory(string CategorykeyIdOrPath, int skip=0, int count=50)
        {
            //TODO: sort product here...
            var categoryid = ServiceProvider.Category(this.Context).GetCategoryId(CategorykeyIdOrPath);  
            var list = ServiceProvider.ProductCategory(this.Context).ProductIdList(categoryid); 
            return this.Repo.Query.WhereIn<Guid>(o => o.Id, list).Skip(skip).Take(count);
        }
         
        public List<Product> Top(int count = 10)
        {
            // TODO: add sort product here. 
            return this.Repo.Query.OrderByDescending(o => o.Order).Take(count); 
        }
    }
}
