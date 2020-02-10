using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class ProductCategoryService : ServiceBase<ProductCategory>
    {

        public List<Guid> FindCategoies(Guid ProductId)
        {
            var allitems = this.Repo.Query.Where(o => o.ProductId == ProductId).SelectAll();

            return allitems.Select(o => o.CategoryId).Distinct().ToList();
        }

        public List<Guid> ProductIdList(Guid CategoryId)
        {
            return this.Repo.Query.Where(o => o.CategoryId == CategoryId).SelectAll().Select(o => o.ProductId).ToList(); 
        }
    }
}
