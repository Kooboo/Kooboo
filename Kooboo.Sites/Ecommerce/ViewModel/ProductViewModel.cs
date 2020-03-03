//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class ProductViewModel : IDynamic
    {

        private RenderContext context { get; set; }

        public ProductViewModel(Product product, RenderContext context, List<Models.ProductProperty> Properties)
        {
            this.context = context;

            this.Id = product.Id;
            this.ProductTypeId = product.ProductTypeId;
            this.UserKey = product.UserKey;
            this.LastModified = product.LastModified;
            this.Online = product.Online;
            this.CreationDate = product.CreationDate;

            var langcontent = product.GetContentStore(context.Culture);
            if (langcontent != null)
            {
                this.Values = langcontent.FieldValues;
            }

            if (Properties != null)
            {
                foreach (var item in Properties.Where(o => !o.IsSystemField && !o.MultipleLanguage))
                {
                    if (!this.Values.ContainsKey(item.Name) || string.IsNullOrEmpty(this.Values[item.Name]))
                    {
                        bool found = false;
                        foreach (var citem in product.Contents)
                        {
                            foreach (var fielditem in citem.FieldValues)
                            {
                                if (fielditem.Key == item.Name)
                                {
                                    this.Values[item.Name] = fielditem.Value;
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                            { break; }
                        }
                    }
                }
            }

        }


        public Guid Id { get; set; }

        public string UserKey { get; set; }

        public Guid ProductTypeId { get; set; }

        public int Order { get; set; }

        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Object GetValue(string FieldName)
        {
            string lower = FieldName.ToLower();

            if (lower == "userkey")
            {
                return this.UserKey;
            }

            else if (lower == "id")
            {
                return this.Id;
            }
            else if (lower == "sequence")
            {
                return this.Order;
            }

            if (Values.ContainsKey(FieldName))
            {
                return Values[FieldName];
            }

            else if (lower == "contenttypeid")
            {
                return this.ProductTypeId;
            }
            else if (lower == "order")
            {
                return this.Order;
            }
            else if (lower == "online")
            {
                return this.Online;
            }
            else if (lower == "lastmodify" || lower == "lastmodified")
            {
                return this.LastModified;
            }
            else if (lower == "creationdate")
            {
                return this.CreationDate;
            }
            return null;
        }

        public void SetValue(string FieldName, object Value)
        {
            this.Values[FieldName] = Value.ToString();
        }

        public Object GetValue(string FieldName, RenderContext Context)
        {
            string culture = Context.Culture;

            var result = GetValue(FieldName);
            if (result == null && Context != null)
            {
                var lower = FieldName.ToLower();
                if (lower == "variants")
                {
                    return this.Variants;
                }

                // check cateogry? 
                else if (lower == "categories")
                {
                    return this.Categories; 
                }
            }

            return result;
        }

        public DateTime LastModified { get; set; }

        public DateTime CreationDate { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public bool Online { get; set; }

        Dictionary<string, object> IDynamic.Values
        {
            get
            {
                return this.Values.ToDictionary(o => o.Key, o => (object)o.Value);
            }
        }

        [JsonIgnore]
        public ProductVariantsViewModel[] Variants
        {
            get
            {
                var variants = ServiceProvider.ProductVariants(this.context).ListByProduct(this.Id);

                return variants.Select(o => new ProductVariantsViewModel(o)).ToArray();
            }
        }


        public CategoryViewModel[] Categories
        {
            get
            {
                var cats = ServiceProvider.Product(this.context).CategoryList(this.Id);

                return cats.Select(o => new CategoryViewModel(o, this.context)).ToArray();
            }
        }
    }
}
