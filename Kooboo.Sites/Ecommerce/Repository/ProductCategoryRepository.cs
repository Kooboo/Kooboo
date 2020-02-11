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
                para.AddColumn<ProductCategory>(o => o.CategoryId); 
                return para;
            }
        }

        public void UpdateCategory(Guid ProductId, List<Guid> CatIds)
        {
            if (CatIds == null)
            {
                CatIds = new List<Guid>(); 
            }

            var olds = this.Query.Where(o => o.ProductId == ProductId).SelectAll();

            foreach (var item in olds)
            {
                if (!CatIds.Contains(item.CategoryId))
                {
                    this.Delete(item.Id);
                }
            }

            foreach (var item in CatIds)
            {
                ProductCategory productcat = new ProductCategory();
                productcat.ProductId = ProductId;
                productcat.CategoryId = item;
                this.AddOrUpdate(productcat); 
            } 
        }

        public List<ProductCategory> GetCatIdByProduct(Guid ProductId)
        {
            
            var list = this.Query.Where(o => o.ProductId == ProductId).SelectAll();

            return list; 
        }  
    } 
}
