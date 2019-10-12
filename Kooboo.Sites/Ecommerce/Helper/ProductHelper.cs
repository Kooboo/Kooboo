//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.Helper
{
    public static class ProductHelper
    {
        public static ProductViewModel ToView(Product product, string lang, List<Models.ProductProperty> Properties)
        {
            if (product == null)
            {
                return null;
            }
            ProductViewModel model = new ProductViewModel();
            model.Id = product.Id;
            model.ProductTypeId = product.ProductTypeId;
            model.UserKey = product.UserKey;
            model.LastModified = product.LastModified;
            model.Online = product.Online;
            model.CreationDate = product.CreationDate;

            var langcontent = product.GetContentStore(lang);
            if (langcontent != null)
            {
                model.Values = langcontent.FieldValues;
            }

            if (Properties != null)
            {
                foreach (var item in Properties.Where(o => !o.IsSystemField && !o.MultipleLanguage))
                {
                    if (!model.Values.ContainsKey(item.Name) || string.IsNullOrEmpty(model.Values[item.Name]))
                    {
                        bool found = false;
                        foreach (var citem in product.Contents)
                        {
                            foreach (var fielditem in citem.FieldValues)
                            {
                                if (fielditem.Key == item.Name)
                                {
                                    model.Values[item.Name] = fielditem.Value;
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

            return model;
        }
    }
}