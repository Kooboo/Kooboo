//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class ViewDataMethodRepository : SiteRepositoryBase<ViewDataMethod>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters param = new ObjectStoreParameters();
                param.AddColumn<ViewDataMethod>(o => o.ViewId);
                param.AddColumn<ViewDataMethod>(o => o.MethodId);
                param.SetPrimaryKeyField<ViewDataMethod>(o => o.Id);
                return param;
            }
        }

        public List<ViewDataMethod> FlatListByView(Guid viewId)
        {
            List<ViewDataMethod> result = new List<ViewDataMethod>();
            var top = this.Query.Where(o => o.ViewId == viewId).SelectAll();

            foreach (var item in top)
            {
                appendMethod(result, item, viewId);
            }
            return result;
        }

        private void appendMethod(List<ViewDataMethod> list, ViewDataMethod current, Guid viewId)
        {
            current.ViewId = viewId;
            list.Add(current);
            foreach (var item in current.Children)
            {
                appendMethod(list, item, viewId);
            }
        }
    }
}