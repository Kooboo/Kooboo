//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class CategoryViewModel
    {
        public RenderContext context { get; set; }

        public CategoryViewModel(Category cat, RenderContext context)
        {
            this.Id = cat.Id;
            this.ParentId = cat.ParentId;
            this.name = cat.Name;
            this.userkey = cat.UserKey;
            var value = cat.GetValue(context.Culture);
            if (value != null)
            {
                this.DisplayName = value.ToString();
            }
            this.context = context;
        }

        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string name { get; set; }

        public string userkey { get; set; }

        public string DisplayName { get; set; }

        public ProductViewModel[] Products(int skip, int take)
        {
            var products = ServiceProvider.Product(this.context).ListByCategory(this.Id, skip, take);

            if (products == null || !products.Any())
            {
                return null;
            }

            var producttype = ServiceProvider.ProductType(context).Get(products[0].ProductTypeId);

            if (producttype != null)
            {
                return products.Select(o => new ProductViewModel(o, this.context.Culture, producttype.Properties)).ToArray();  
            }

            return null;
        }

    }
}
