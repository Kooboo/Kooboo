//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.ViewModel;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.Repository
{

    public class ProductRepository : SiteRepositoryBase<Product>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<Product>(it => it.Id);   
                paras.AddColumn<Product>(it => it.Online);
                paras.AddColumn<Product>(it => it.LastModified);
                paras.SetPrimaryKeyField<Product>(o => o.Id);
                return paras;
            }
        }

        public override bool AddOrUpdate(Product product, Guid UserId = default(Guid))
        {
            if (string.IsNullOrWhiteSpace(product.UserKey))
            {
                product.UserKey = this.GenerateUserKey(product);
            }
            return base.AddOrUpdate(product, UserId);
        }

        public  ProductViewModel GetView(Guid id, string lang)
        {
            return GetView(this.Get(id), lang);
        }

        public ViewModel.ProductViewModel GetView(Product product, string lang)
        {
            if (product != null)
            {
                var prop = this.SiteDb.GetSiteRepository<ProductTypeRepository>().GetColumns(product.ProductTypeId);
                return new ProductViewModel(product, lang, prop);
            }
            return null;
        }

        // get the default content item...search for all possible text repositories..
        //public TextContentViewModel GetDefaultContentFromFolder(Guid FolderId, string CurrentCulture = null)
        //{
        //    if (string.IsNullOrWhiteSpace(CurrentCulture))
        //    {
        //        CurrentCulture = this.WebSite.DefaultCulture;
        //    }

        //    var list = this.Query.Where(o => o.FolderId == FolderId).Take(10);

        //    foreach (var item in list.Where(o => o.Online))
        //    {
        //        var view = GetView(item, CurrentCulture);
        //        if (view != null && view.Values.Count() > 0)
        //        {
        //            return view;
        //        }
        //    }

        //    foreach (var item in list.Where(o => !o.Online))
        //    {
        //        var view = GetView(item, CurrentCulture);
        //        if (view != null && view.Values.Count() > 0)
        //        {
        //            return view;
        //        }
        //    }

        //    return null;
        //}

        public string GenerateUserKey(Product proudct)
        {
            string lang = this.SiteDb.WebSite.DefaultCulture;
            var producttype = this.SiteDb.ContentTypes.Get(proudct.ProductTypeId);
            if (producttype != null)
            {
                object value = null;
                foreach (var item in producttype.Properties.Where(o => o.IsSummaryField))
                {
                    value = proudct.GetValue(item.Name, lang);
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return GetUserKey(value.ToString());
                    }
                }

                foreach (var item in producttype.Properties.Where(o => o.DataType == Data.Definition.DataTypes.String))
                {
                    value = proudct.GetValue(item.Name, lang);
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return GetUserKey(value.ToString());
                    }
                }

                foreach (var item in producttype.Properties.Where(o => !o.IsSummaryField && !o.IsSystemField))
                {
                    value = proudct.GetValue(item.Name, lang);
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return GetUserKey(value.ToString());
                    }
                }
            }

            foreach (var item in proudct.Contents)
            {
                foreach (var field in item.FieldValues)
                {
                    if (!string.IsNullOrEmpty(field.Value))
                    {
                        return GetUserKey(field.Value);
                    }
                }
            }

            return Guid.NewGuid().ToString();
        }

        private string GetUserKey(string key)
        {
            key = UserKeyHelper.ToSafeUserKey(key);
            if (!IsUserKeyExists(key))
            {
                return key;
            }
            string newkey = string.Empty;
            for (int i = 0; i < 99; i++)
            {
                newkey = key + i.ToString();
                if (!IsUserKeyExists(newkey))
                {
                    return newkey;
                }
            }
            Random rnd = new Random();
            for (int i = 0; i < 9999; i++)
            {
                newkey = key + rnd.Next(i, int.MaxValue).ToString();
                if (!IsUserKeyExists(newkey))
                {
                    return newkey;
                }
            }
            return null;
        }

        internal bool IsUserKeyExists(string userKey)
        {
            if (string.IsNullOrEmpty(userKey))
            { return true; }
            Guid id = Lib.Security.Hash.ComputeGuidIgnoreCase(userKey);
            return this.Get(id) != null;
        }

        //public void EusureNonLangContent(TextContent content, ContentType contenttype = null)
        //{
        //    if (contenttype == null)
        //    {
        //        contenttype = this.SiteDb.ContentTypes.Get(content.ContentTypeId);
        //    }
                  
        //    string defaultculture = this.SiteDb.WebSite.DefaultCulture;

        //    var NoSysNoMul = contenttype.Properties.Where(o => o.IsSystemField == false && o.MultipleLanguage == false).ToList();

        //    foreach (var item in NoSysNoMul)
        //    {
        //        string value = null;
        //        var langstore = content.GetContentStore(defaultculture);
        //        if (langstore != null && langstore.FieldValues.ContainsKey(item.Name))
        //        {
        //            value = langstore.FieldValues[item.Name];
        //        }

        //        if (string.IsNullOrEmpty(value))
        //        {
        //            foreach (var lang in content.Contents)
        //            {
        //                if (lang.FieldValues.ContainsKey(item.Name))
        //                {
        //                    value = lang.FieldValues[item.Name];
        //                    break;
        //                }
        //            }
        //        }

        //        bool valueset = false;
        //        // remove the key...  
        //        foreach (var citem in content.Contents)
        //        {
        //            if (citem.Lang != defaultculture)
        //            {
        //                citem.FieldValues.Remove(item.Name);
        //            }
        //            else
        //            {
        //                citem.FieldValues[item.Name] = value;
        //                valueset = true;
        //            }
        //        }

        //        if (!valueset)
        //        {
        //            content.SetValue(item.Name, value, defaultculture);
        //        }

        //    }
        //}


        public void UpdateVariants(Guid ProductId,  List<ProductVariants> variants)
        {    
            if (variants == null)
            {
                variants = new List<ProductVariants>(); 
            }

            foreach (var item in variants)
            {
                item.ProductId = ProductId;
                item.Id = default(Guid); // reset id. 
            }

            var repo = this.SiteDb.GetSiteRepository<ProductVariantsRepository>(); 

            var old = repo.ListByProductId(ProductId);

            foreach (var item in old)
            {
                if (variants.Find(o=>o.Id == item.Id) == null)
                {
                    repo.Delete(item.Id); 
                }
            }

            foreach (var item in variants)
            {
                repo.AddOrUpdate(item); 
            }       

        }


        public List<Product> GetByCategory(Guid CategoryId)
        {                                                  
            var allcats = GetAllSubCats(CategoryId).ToList();

            var allproductcats = this.SiteDb.GetSiteRepository<ProductCategoryRepository>().List();


            var allproductids = allproductcats.Where(o => allcats.Contains(o.CategoryId)).Select(o=>o.ProductId).ToList();

            return this.Query.WhereIn<Guid>(o => o.Id, allproductids.ToList()).SelectAll(); 
         
        }

        private HashSet<Guid> GetAllSubCats(Guid Id)
        {
            HashSet<Guid> result = new HashSet<Guid>();
            var all = this.SiteDb.GetSiteRepository<CategoryRepository>().All();
            result.Add(Id);
            SetSubs(all, Id, ref result);
            return result;
        }

        private void SetSubs(List<Ecommerce.Models.Category> all, Guid ParentId, ref HashSet<Guid> result)
        {
            var subs = all.Where(o => o.ParentId == ParentId).ToList();
            foreach (var item in subs)
            {
                result.Add(item.Id);
                SetSubs(all, item.Id, ref result);
            }
        }
    }


}
