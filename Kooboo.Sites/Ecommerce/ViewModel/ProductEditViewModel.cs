//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
                             
    public class ProductEditViewModel
    {
        public List<ProductFieldViewModel> Properties { get; set; }

        public List<ProductVariants> Variants { get; set; }

        public List<ProductCategory> Categories { get; set; }
                  
        public Guid ProductTypeId { get; set; }

    }






}
