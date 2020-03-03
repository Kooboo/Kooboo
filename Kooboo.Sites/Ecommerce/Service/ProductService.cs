using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class ProductService : ServiceBase<Product>
    {
        public List<Product> ByCategory(string CategorykeyIdOrPath, int skip = 0, int count = 50)
        {
            //TODO: sort product here...
            var category = ServiceProvider.Category(this.Context).Get(CategorykeyIdOrPath);
            if (category != null)
            {
                var list = ServiceProvider.ProductCategory(this.Context).ProductIdList(category.Id);
                return this.Repo.Query.WhereIn<Guid>(o => o.Id, list).Skip(skip).Take(count);
            }
            return new List<Product>();
        }

        public List<Product> Top(int count = 10)
        {
            // TODO: add sort product here. 
            return this.Repo.Query.OrderByDescending(o => o.Order).Take(count);
        }

        public List<Category> CategoryList(Guid ProductId)
        {
            var cates = ServiceProvider.ProductCategory(this.Context).FindCategoies(ProductId);

            List<Category> result = new List<Category>();

            foreach (var item in cates)
            {
                var cat = ServiceProvider.Category(this.Context).Get(item);
                if (cat != null)
                {
                    result.Add(cat);
                }
            }

            return result; 
        }
    }
}
