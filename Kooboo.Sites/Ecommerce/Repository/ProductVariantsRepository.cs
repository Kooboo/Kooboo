//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.Repository
{
    public class ProductVariantsRepository : SiteRepositoryBase<ProductVariants>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<ProductVariants>(it => it.Id);
                paras.AddColumn<ProductVariants>(it => it.Online);
                paras.AddColumn<ProductVariants>(it => it.LastModified);
                paras.AddIndex<ProductVariants>(it => it.ProductId);
                paras.SetPrimaryKeyField<ProductVariants>(o => o.Id);
                return paras;
            }
        }

        public List<ProductVariants> ListByProductId(Guid productId)
        {
            var list = this.Query.Where(o => o.ProductId == productId).SelectAll();
            return list;
        }
    }
}