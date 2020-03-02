using Kooboo.Data.Attributes;
using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class ProductVariantsViewModel
    {
        public ProductVariantsViewModel(ProductVariants variants)
        {
            this.Id = variants.Id;
            this.Sku = variants.Sku;
            this.ProductId = variants.ProductId;
            this.Price = variants.Price;
            this.Groups = variants.Groups;
            this.Spec = variants.Variants;
            this.Images = variants.Images;
            this.Thumbnail = variants.Thumbnail;
            this.Stock = variants.Stock;
            this.Order = variants.Order; 
        }

        public Guid Id
        {
            get; set;
        }

        public string Sku { get; set; }

        public Guid ProductId { get; set; }

        // can have additional things to group  
        public List<Guid> Groups { get; set; }

        public Dictionary<String, string> Spec { get; set; }

        [KIgnore]
        public Dictionary<String, string> Specification
        {
            get
            {
                return this.Spec;
            }
        }

        public List<string> Images { get; set; }

        public string Thumbnail { get; set; }

        public int Stock { get; set; }

        public decimal Price { get; set; }

        public int Order { get; set; }
    }
}
