//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System.Linq;

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

        public List<Category> RootCategories()
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
           
    }


}
