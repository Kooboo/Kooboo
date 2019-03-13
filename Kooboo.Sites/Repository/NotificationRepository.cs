//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Models;
using Kooboo.IndexedDB;

namespace Kooboo.Sites.Repository
{
   public class NotificationRepository : SiteRepositoryBase<Notification>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters param = new ObjectStoreParameters();
                param.AddColumn<Notification>(o => o.IsRead);
                param.AddIndex<Notification>(o => o.LastModifyTick);
                param.SetPrimaryKeyField<Notification>(o => o.Id);
                return param;
            }
        }
    }
}
