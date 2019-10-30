//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class ContentCategoryRepository : SiteRepositoryBase<ContentCategory>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<ContentCategory>(it => it.Id);
                paras.AddColumn<ContentCategory>(it => it.CategoryFolder);
                paras.AddColumn<ContentCategory>(it => it.CategoryId);
                paras.AddIndex<ContentCategory>(it => it.ContentId);
                paras.SetPrimaryKeyField<ContentCategory>(o => o.Id);
                return paras;
            }
        }

        public List<ContentCategory> GetCategories(Guid folderId, Guid contentId)
        {
            return Query.Where(it => it.ContentId == contentId && it.CategoryFolder == folderId).SelectAll();
        }

        public void UpdateCategory(Guid contentId, Guid folderId, List<Guid> categoryIds, Guid userId)
        {
            List<ContentCategory> categories = new List<ContentCategory>();
            foreach (var item in categoryIds)
            {
                ContentCategory category = new ContentCategory
                {
                    ContentId = contentId, CategoryFolder = folderId, CategoryId = item
                };
                categories.Add(category);
            }

            var currentresult = this.Query.Where(o => o.ContentId == contentId && o.CategoryFolder == folderId).SelectAll();

            foreach (var item in currentresult)
            {
                if (!categories.Any(o => o.Id == item.Id))
                {
                    this.Delete(item.Id);
                }
            }

            foreach (var item in categories)
            {
                this.AddOrUpdate(item);
            }
        }
    }
}