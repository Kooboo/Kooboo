using Kooboo.Data.Context;
// using Kooboo.Sites.Ecommerce.KScript.Model;
using Kooboo.Sites.Ecommerce.Repository;
using Kooboo.Sites.Ecommerce.ViewModel;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Sites.Ecommerce.Service;
using Kooboo.Sites.Ecommerce;

namespace KScript.Ecommerce
{
    public class KCategory
    {
        private RenderContext context { get; set; }

        private CategoryService service { get; set; }

        public KCategory(RenderContext context)
        {
            this.context = context;
            this.service = Kooboo.Sites.Ecommerce.ServiceProvider.GetService<CategoryService>(this.context);
        }

        private List<CategoryViewModel> SubCategories(Guid ParentId)
        {
            var subcates = context.WebSite.SiteDb().GetSiteRepository<CategoryRepository>().AllCategories(ParentId);

            List<CategoryViewModel> result = new List<CategoryViewModel>();

            foreach (var item in subcates)
            {
                var view = new Kooboo.Sites.Ecommerce.ViewModel.CategoryViewModel(item, this.context);
                if (view != null)
                {
                    result.Add(view);
                }
            }
            return result;
        }

        public CategoryViewModel[] Top()
        {
            var subcates = service.Top();

            return subcates.Select(o => new CategoryViewModel(o, this.context)).ToArray();
        }

        public CategoryViewModel[] Sub(string ParentKeyOrPath)
        {
            var subs = service.Sub(ParentKeyOrPath);
            return subs.Select(o => new CategoryViewModel(o, this.context)).ToArray();
        }

        public ProductViewModel[] ProductList(string CatNameIdOrPath, int skip, int take)
        {
            var products = ServiceProvider.Product(this.context).ByCategory(CatNameIdOrPath, skip, take);

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

        public CategoryViewModel Get(string NameKeyIdOrPath)
        {
            var cat = this.service.Get(NameKeyIdOrPath);
            if (cat != null)
            {
                return new CategoryViewModel(cat, this.context);
            }
            return null;
        }
    }
}
