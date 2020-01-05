using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace KScript.Model
{
    public class CategoryView
    {
        internal RenderContext context { get; set; }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }

       public  List<ProductView> Products
        {
            get
            {
                return null; 
            }
        }

        

    }
}
