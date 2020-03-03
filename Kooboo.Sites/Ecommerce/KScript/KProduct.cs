using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Service;
using Kooboo.Sites.Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Ecommerce.KScript
{

    public class KProduct
    {
        private RenderContext context { get; set; }

        private ProductService service { get; set; }

        public KProduct(RenderContext context)
        {
            this.context = context;
            this.service = Kooboo.Sites.Ecommerce.ServiceProvider.GetService<ProductService>(this.context);
        }

        public ProductViewModel[] Top(int count)
        {
            var products = service.Top(count);

            return products.Select(o => new ProductViewModel(o, this.context, null)).ToArray();
        }

        public ProductViewModel Get(string keyorid)
        {
            var p = this.service.Get(keyorid);

            if (p != null)
            {
                var ptype = ServiceProvider.ProductType(this.context).Get(p.ProductTypeId);
                if (ptype != null)
                {
                    return new ProductViewModel(p, this.context, ptype.Properties);
                }

            }
            return null;
        }

        public ProductViewModel[] ByCategory(string CatNameIdOrPath, int skip, int take)
        {
            var products = this.service.ByCategory(CatNameIdOrPath, skip, take);

            if (products == null || !products.Any())
            {
                return null;
            }

            var producttype = ServiceProvider.ProductType(context).Get(products[0].ProductTypeId);

            if (producttype != null)
            {
                return products.Select(o => new ProductViewModel(o, this.context, producttype.Properties)).ToArray();
            }

            return null;
        }


    }





}
