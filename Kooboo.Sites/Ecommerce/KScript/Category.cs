using Kooboo.Data.Context;
// using Kooboo.Sites.Ecommerce.KScript.Model;
using Kooboo.Sites.Ecommerce.Repository;
using Kooboo.Sites.Extensions;
using KScript.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace  KScript.Ecommerce
{
  public class KCategory
    {
        private RenderContext context { get; set; }
        
        public KCategory(RenderContext context)
        {
            this.context = context; 
        }

        public List<CategoryView> SubCategories(Guid ParentId)
        {
            var subcates = context.WebSite.SiteDb().GetSiteRepository<CategoryRepository>().AllCategories(ParentId);

            List<CategoryView> result = new List<CategoryView>();

            foreach (var item in subcates)
            {
                var view = KScript.Translator.ToCategoryView(item, context); 
                if (view !=null)
                {
                    result.Add(view); 
                }
            }

            return result;
        }



    }
}
