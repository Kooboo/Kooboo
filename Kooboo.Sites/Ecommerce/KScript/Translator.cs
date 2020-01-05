using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using KScript.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace  KScript
{
  public static class Translator
    {
        public static CategoryView ToCategoryView(Category cat, RenderContext Context)
        {
            if (cat == null)
            {
                return null;
            }
            CategoryView model = new CategoryView();
            model.Id = cat.Id;
            model.context = Context;
            model.ParentId = cat.ParentId;
            var objname = cat.GetValue(Context.Culture); 
            
            if (objname !=null)
            {
                model.Name = objname.ToString(); 
            } 
            return model;
        }


        public static ProductView ToProductView(Product product, RenderContext context, List<ProductProperty> properties)
        {
            return null; 
        }
    }
}
