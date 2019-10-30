//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.Repository
{
    public class ProductCategoryRepository : SiteRepositoryBase<ProductCategory>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddIndex<ProductCategory>(o => o.ProductId);
                para.AddIndex<ProductCategory>(o => o.CategoryId);
                return para;
            }
        }

        public void UpdateCategory(Guid productId, List<Guid> catIds)
        {
            if (catIds == null)
            {
                catIds = new List<Guid>();
            }

            var olds = this.Query.Where(o => o.ProductId == productId).SelectAll();

            foreach (var item in olds)
            {
                if (!catIds.Contains(item.CategoryId))
                {
                    this.Delete(item.Id);
                }
            }

            foreach (var item in catIds)
            {
                ProductCategory productcat = new ProductCategory {ProductId = productId, CategoryId = item};
                this.AddOrUpdate(productcat);
            }
        }

        public List<ProductCategory> GetCatIdByProduct(Guid productId)
        {
            var list = this.Query.Where(o => o.ProductId == productId).SelectAll();

            return list;
        }
    }
}