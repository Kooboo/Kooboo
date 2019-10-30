//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class ViewRepository : SiteRepositoryBase<View>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var para = new ObjectStoreParameters();
                para.AddColumn("Name", 100);
                para.AddColumn<View>(o => o.LastModified);
                para.AddColumn<View>(o => o.ModuleId);
                para.SetPrimaryKeyField<View>(o => o.Id);
                return para;
            }
        }

        public void UpdateDataSources(Guid viewId, IEnumerable<ViewDataMethod> viewDataMethods, Guid userId = default(Guid))
        {
            foreach (var item in viewDataMethods)
            {
                item.ViewId = viewId;
            }

            var oldlist = this.SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == viewId).SelectAll();

            var oldguidlist = oldlist.Select(o => o.Id).ToList();

            var newguidlist = viewDataMethods.Select(o => o.Id).ToList();

            foreach (var item in oldguidlist)
            {
                if (!newguidlist.Contains(item))
                {
                    this.SiteDb.ViewDataMethods.Delete(item, userId);
                }
            }
            foreach (var item in viewDataMethods)
            {
                this.SiteDb.ViewDataMethods.AddOrUpdate(item, userId);
            }
        }
    }
}