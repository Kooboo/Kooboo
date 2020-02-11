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

namespace KScript.Ecommerce
{
    public class KCategory
    {
        private RenderContext context { get; set; }

        public KCategory(RenderContext context)
        {
            this.context = context;
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
         
        public CategoryViewModel[] TopCategories()
        {
            var catservice = Kooboo.Sites.Ecommerce.ServiceProvider.GetService<CategoryService>(this.context); 
             
            var subcates = context.WebSite.SiteDb().GetSiteRepository<CategoryRepository>().AllCategories();

            return subcates.Select(o => new CategoryViewModel(o, this.context)).ToArray(); 
        }
            
    }
}
