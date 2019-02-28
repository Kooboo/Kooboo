//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.ViewModel;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation.Ecommerce
{
    public class ProductApi : SiteObjectApi<Product>
    {
        public Guid Post(ProductUpdateViewModel model, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var producttypeid = call.GetValue<Guid>("type");

            var producttype = sitedb.ProductType.Get(producttypeid);
            if (producttype == null || producttypeid == default(Guid))
            { return default(Guid); }

            string userkey = ExtraValue(model, "userkey");

            if (!string.IsNullOrEmpty(userkey))
            {
                userkey = Kooboo.Sites.Contents.UserKeyHelper.ToSafeUserKey(userkey);
            }

            string stronline = ExtraValue(model, "online");
            bool online = true;
            if (!string.IsNullOrEmpty(stronline) && stronline.ToLower() == "false")
            {
                online = false;
            }

            int sequence = 0;
            var strsequence = ExtraValue(model, "sequence");
            if (!string.IsNullOrEmpty(strsequence))
            {
                int.TryParse(strsequence, out sequence);
            }

            Product newproduct = sitedb.Product.Get(call.ObjectId);

            if (newproduct == null)
            {
                newproduct = new Product() { ProductTypeId = producttypeid, UserKey = userkey };
                if (!string.IsNullOrEmpty(userkey) && sitedb.TextContent.IsUserKeyExists(userkey))
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("UserKey has been taken", call.Context));
                }
            }

            if (!string.IsNullOrEmpty(userkey) && newproduct.UserKey != userkey)
            {
                sitedb.Product.Delete(newproduct.Id);
                newproduct.UserKey = userkey;
            }

            newproduct.Online = online;
            newproduct.Order = sequence;

            foreach (var item in producttype.Properties.Where(o => !o.MultipleLanguage))
            {
                var value = ExtraValue(model, item.Name);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    newproduct.SetValue(item.Name, value, call.WebSite.DefaultCulture);
                }
            }

            foreach (var langDict in model.Values)
            {
                string lang = langDict.Key;
                foreach (var item in langDict.Value)
                {
                    string value = item.Value == null ? string.Empty : item.Value.ToString();
                    newproduct.SetValue(item.Key, value, lang);
                }
            }

            // sitedb.TextContent.EusureNonLangContent(newcontent, contenttype);    
            sitedb.Product.AddOrUpdate(newproduct, call.Context.User.Id);

            sitedb.Product.UpdateVariants(newproduct.Id, model.Variants);

            sitedb.ProductCategory.UpdateCategory(newproduct.Id, model.Categories);

            return newproduct.Id;
        }

        [Attributes.RequireParameters("FolderId")]
        public List<ContentFieldViewModel> GetFields(ApiCall call)
        {
            Guid FolderId = call.GetGuidValue("FolderId");

            if (FolderId != default(Guid))
            {
                var contenttype = call.WebSite.SiteDb().ContentTypes.GetByFolder(FolderId);
                List<ContentFieldViewModel> result = new List<ContentFieldViewModel>();

                foreach (var item in contenttype.Properties)
                {
                    if (item.Editable)
                    {
                        ContentFieldViewModel model = new ContentFieldViewModel();
                        model.Name = item.Name;
                        model.Validations = item.Validations;
                        model.ControlType = item.ControlType;
                        model.ToolTip = item.Tooltip;
                        model.Order = item.Order;
                        // model.Value = content.GetValue(item.Name);
                        model.IsMultilingual = item.MultipleLanguage;
                        model.MultipleValue = item.MultipleValue;
                        model.selectionOptions = item.selectionOptions;
                        result.Add(model);
                    }
                }
                return result;
            }
            return null;
        }


        public PagedListViewModel<ProductViewModel> ProductList(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            int pagesize = ApiHelper.GetPageSize(call, 50);
            int pagenr = ApiHelper.GetPageNr(call);

            string language = string.IsNullOrEmpty(call.Context.Culture) ? call.WebSite.DefaultCulture : call.Context.Culture;

            PagedListViewModel<ProductViewModel> model = new PagedListViewModel<ProductViewModel>();
            model.PageNr = pagenr;
            model.PageSize = pagesize;

            var products = sitedb.Product.All();

            model.TotalCount = products.Count();
            model.TotalPages = ApiHelper.GetPageCount(model.TotalCount, model.PageSize);

            var productlist = products.OrderByDescending(o => o.LastModified).Skip(model.PageNr * model.PageSize - model.PageSize).Take(model.PageSize).ToList();

            model.List = new List<ProductViewModel>();

            foreach (var item in productlist)
            {
                var type = sitedb.ProductType.Get(item.ProductTypeId);


                model.List.Add(Kooboo.Sites.Ecommerce.Helper.ProductHelper.ToView(item, language, type != null ? type.Properties : null));
            }

            return model;
        }

        private List<ProductFieldViewModel> GetProperties(ApiCall call, Guid ProductTypeId, Product product)
        {
            var culture = call.WebSite.DefaultCulture;
            var sitedb = call.Context.WebSite.SiteDb();

            List<ProductFieldViewModel> result = new List<ProductFieldViewModel>();

            var ProductType = sitedb.ProductType.Get(ProductTypeId);

            bool online = true;

            if (product == null)
            {
                foreach (var item in ProductType.Properties)
                {
                    if (item.Editable)
                    {
                        ProductFieldViewModel model = new ProductFieldViewModel();
                        model.Name = item.Name;
                        model.DisplayName = item.DisplayName;
                        model.Validations = item.Validations;
                        model.ControlType = item.ControlType;
                        model.ToolTip = item.Tooltip;
                        model.Order = item.Order;
                        model.IsSpecification = item.IsSpecification;

                        if (item.MultipleLanguage)
                        {
                            foreach (var cultureitem in call.WebSite.Culture.Keys.ToList())
                            {
                                model.Values.Add(cultureitem, "");
                            }
                        }
                        else
                        {
                            if (item.DataType == Data.Definition.DataTypes.Bool)
                            {
                                model.Values.Add(call.WebSite.DefaultCulture, "true");
                            }
                            else
                            {
                                model.Values.Add(call.WebSite.DefaultCulture, "");
                            }
                        }
                        model.IsMultilingual = item.MultipleLanguage;
                        model.selectionOptions = item.selectionOptions;
                        model.MultipleValue = item.MultipleValue;

                        result.Add(model);
                    }
                }
            }
            else
            {
                online = product.Online;

                foreach (var item in ProductType.Properties)
                {
                    if (item.Editable)
                    {
                        ProductFieldViewModel model = new ProductFieldViewModel();
                        model.Name = item.Name;
                        model.Validations = item.Validations;
                        model.ControlType = item.ControlType;
                        model.DisplayName = item.DisplayName;
                        model.IsMultilingual = item.MultipleLanguage;
                        model.ToolTip = item.Tooltip;
                        model.Order = item.Order;
                        model.IsSpecification = item.IsSpecification;
                        model.selectionOptions = item.selectionOptions;
                        model.MultipleValue = item.MultipleValue;

                        if (item.MultipleLanguage)
                        {
                            foreach (var lang in product.Contents)
                            {
                                var itemvalue = product.GetValue(model.Name, lang.Lang);
                                model.Values[lang.Lang] = itemvalue != null ? itemvalue.ToString() : string.Empty;
                            }
                            foreach (var sitelang in call.WebSite.Culture.Keys.ToList())
                            {
                                if (!model.Values.ContainsKey(sitelang))
                                {
                                    model.Values.Add(sitelang, "");
                                }
                            }
                        }
                        else
                        {
                            var itemvalue = product.GetValue(model.Name, culture);
                            model.Values[culture] = itemvalue != null ? itemvalue.ToString() : null;
                        }
                        result.Add(model);
                    }
                }
            }

            var onlineitem = result.Find(o => o.Name.ToLower() == "online");
            if (onlineitem != null)
            {
                result.Remove(onlineitem);

                onlineitem.ControlType = "boolean";
                result.Add(onlineitem);
            }

            return result;
        }

        public ProductEditViewModel GetEdit(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var productid = call.GetValue<Guid>("id");
            Guid ProductTypeId = call.GetValue<Guid>("producttypeId");
            Product product = null;

            var model = new ProductEditViewModel();

            if (productid != default(Guid))
            {
                product = sitedb.Product.Get(productid);
                ProductTypeId = product.ProductTypeId;
                model.Categories = sitedb.ProductCategory.GetCatIdByProduct(productid).ToList();

                model.Variants = sitedb.ProductVariants.ListByProductId(productid).ToList();   
            }

            model.Properties = GetProperties(call, ProductTypeId, product).OrderBy(o => o.Order).ToList();

            return model;
        }

        public string ExtraValue(ProductUpdateViewModel updatemodel, string FieldName)
        {
            if (string.IsNullOrWhiteSpace(FieldName))
            {
                return null;
            }
            FieldName = FieldName.ToLower();

            string key = null;

            foreach (var langitem in updatemodel.Values)
            {
                foreach (var fielditem in langitem.Value)
                {
                    if (fielditem.Key.ToLower() == FieldName)
                    {
                        key = fielditem.Value;
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                // remove the key 
                foreach (var langitem in updatemodel.Values)
                {
                    List<string> keysToRemove = new List<string>();

                    foreach (var fielditem in langitem.Value)
                    {
                        if (fielditem.Key.ToLower() == FieldName)
                        {
                            keysToRemove.Add(fielditem.Key);
                        }
                    }

                    foreach (var item in keysToRemove)
                    {
                        langitem.Value.Remove(item);
                    }
                }
            }
            return key;
        }

    }
}
