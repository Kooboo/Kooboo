//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Repository
{
    public class FormValueRepository : SiteRepositoryBase<FormValue>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var storeParams = new ObjectStoreParameters();
                storeParams.AddColumn<FormValue>(x => x.FormId);
                storeParams.AddIndex<FormValue>(o => o.LastModifyTick);
                storeParams.SetPrimaryKeyField<FormValue>(o => o.Id); 
                return storeParams;
            }
        }

    }
}
