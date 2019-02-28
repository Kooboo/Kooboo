//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.Models
{
 
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    [Kooboo.Attributes.NameAsID]
    public class ProductType : Kooboo.Sites.Models.CoreObject
    {
        public ProductType()
        {
            this.ConstType = ConstObjectType.ProductType; 
        }
    
        private List<ProductProperty> _properties;
        public List<ProductProperty> Properties
        {
            get
            {
                if (_properties == null)
                { _properties = new List<ProductProperty>(); }
                return _properties;
            }
            set
            { _properties = value; }
        }

        public ProductProperty GetProperty(string propertyName)
        {
            return Properties.FirstOrDefault(it => it.Name.Equals(propertyName, System.StringComparison.OrdinalIgnoreCase));
        }

        public override int GetHashCode()
        {
            string unique = ""; 
            if (_properties != null)
            {
                foreach (var item in this.Properties)
                {
                    unique += item.GetHashCode().ToString();
                }
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }











}
