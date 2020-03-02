//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System.Linq;
using Kooboo.Sites.Contents;

namespace Kooboo.Sites.Ecommerce.Repository
{
    public class CategoryRepository : SiteRepositoryBase<Category>
    { 
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddColumn<Category>(o => o.ParentId);
                return para;
            }
        }

        public override bool AddOrUpdate(Category cat, Guid UserId = default(Guid))
        {
            if (string.IsNullOrWhiteSpace(cat.UserKey))
            {
                cat.UserKey = this.GenerateUserKey(cat);
            }
            return base.AddOrUpdate(cat, UserId);
        }

        public override void Delete(Guid id)
        {
            var allcate = this.SiteDb.GetSiteRepository<CategoryRepository>().All();
            _delete(id, ref allcate);
        }

        private void _delete(Guid id, ref List<Category> all)
        {
            var subs = all.Where(o => o.ParentId == id);

            foreach (var item in subs)
            {
                _delete(item.Id, ref all);
            }
            base.Delete(id);
        }

        public List<Category> TopCategories()
        {
            return this.Query.Where(o => o.ParentId == default(Guid)).SelectAll();
        }

        public List<Category> AllCategories(Guid ParentId = default(Guid))
        {
            var all = this.All();

            if (ParentId == default(Guid))
            {
                return all;
            }
            else
            {
                var result = new List<Category>();
                Filtersub(all, ParentId, ref result);
                return result;
            }
        }

        private void Filtersub(List<Category> All, Guid ParentId, ref List<Category> result)
        {
            var directSub = All.Where(o => o.ParentId == ParentId);

            foreach (var item in directSub)
            {
                result.Add(item);
                Filtersub(All, item.Id, ref result);
            }
        }
         
        public string GenerateUserKey(Category category)
        {
            if (category == null)
            {
                return System.Guid.NewGuid().ToString();
            }
            
            if  (Lib.Helper.CharHelper.isAsciiDigit(category.Name))
            {
                return GetUserKey(category.Name); 
            }

            // try use default or english.
            if (category.Values.ContainsKey("en"))
            {
                var value = category.Values["en"];
                if (value !=null)
                {
                    return GetUserKey(value.ToString());
                } 
            }
            string lang = this.SiteDb.WebSite.DefaultCulture;
            if (category.Values.ContainsKey(lang))
            {
                var value = category.Values[lang];
                if (value != null)
                {
                    return GetUserKey(value.ToString());
                }
            }

            return GetUserKey(category.Name);  
            //foreach (var item in category.Values)
            //{  
            //     if (item.Value !=null)
            //    {
            //        return GetUserKey(item.Value.ToString()); 
            //    }
            //} 
            // return Guid.NewGuid().ToString();
        }

        private string GetUserKey(string key)
        {
            key = UserKeyHelper.ToSafeUserKey(key);
            if (!IsUserKeyExists(key))
            {
                return key;
            }
            string newkey = string.Empty;
            for (int i = 2; i < 99; i++)
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
         

    }


}
