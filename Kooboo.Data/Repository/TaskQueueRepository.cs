//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Repository
{
    public class TaskQueueRepository : RepositoryBase<Queue>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<Queue>(o => o.WebSiteId);
                paras.SetPrimaryKeyField<Queue>(o => o.Id); 
                return paras;
            }
        }
    } 
}
