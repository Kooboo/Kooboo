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

        public ProductViewModel[] Top()
        {
            var products = service.Top();

            return products.Select(o => new ProductViewModel(o, this.context, null)).ToArray();
        }

        public CategoryViewModel[] Sub(string ParentKeyOrPath)
        {
            var subs = service.Sub(ParentKeyOrPath);
            return subs.Select(o => new CategoryViewModel(o, this.context)).ToArray();
        }

    }





}
