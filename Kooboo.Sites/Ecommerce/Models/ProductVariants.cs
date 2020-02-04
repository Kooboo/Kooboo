//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic; 

namespace Kooboo.Sites.Ecommerce.Models
{ 
    public class ProductVariants : CoreObject
    {
        public ProductVariants()
        {
            this.ConstType = ConstObjectType.ProductVariants; 
        }

        private Guid _id;

        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.ProductId.ToString();
                    if (this.Variants != null)
                    {
                        foreach (var item in Variants)
                        {
                            unique += item.Key + item.Value;
                        }
                    }
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique);
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Sku { get; set; }

        public Guid ProductId { get; set; }

        // can have additional things to group  
        public List<Guid> Groups { get; set; }

        public Dictionary<String, string> Variants { get; set; }

        ///public List<Variants> Variants { get; set; } 
        public List<string> Images { get; set; }

        public string Thumbnail { get; set; }

        public int Stock { get; set; }

        public decimal Price { get; set; }

        public int Order { get; set; }

        public override int GetHashCode()
        {
            string unique = this.Images + this.Price.ToString() + this.Thumbnail + this.Stock.ToString(); 
            unique += this.Sku + this.Order.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }

    public class Variants
    {
        public string SpecificationPropertyName { get; set; }

        public string Value { get; set; }
    }


    public enum VariantType
    {
        Normal = 0,
        Group = 1
    }
}
