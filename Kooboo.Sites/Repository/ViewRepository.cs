//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Extensions;
using Kooboo.IndexedDB;

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

        public  void UpdateDataSources( Guid ViewId, IEnumerable<ViewDataMethod> ViewDataMethods, Guid UserId = default(Guid))
        {
            foreach (var item in ViewDataMethods)
            {
                item.ViewId = ViewId; 
            }

            var oldlist = this.SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == ViewId).SelectAll();

            var oldguidlist = oldlist.Select(o => o.Id).ToList();

            var newguidlist = ViewDataMethods.Select(o => o.Id).ToList();

            foreach (var item in oldguidlist)
            {
                if (!newguidlist.Contains(item))
                {
                   this.SiteDb.ViewDataMethods.Delete(item, UserId);
                }
            } 
            foreach (var item in ViewDataMethods)
            { 
                this.SiteDb.ViewDataMethods.AddOrUpdate(item, UserId);
            } 
        }
    }
}
