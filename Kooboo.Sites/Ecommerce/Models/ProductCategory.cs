//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class ProductCategory : Kooboo.Sites.Models.CoreObject
    {
        public ProductCategory()
        {
            this.ConstType = ConstObjectType.ProductCategory; 
        }

        private Guid _id; 

        public override Guid Id {   
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.ProductId.ToString() + this.CategoryId.ToString();
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique); 
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }  
        }

        public Guid ProductId { get; set; }
         

        public Guid CategoryId { get; set; }

    }
}
