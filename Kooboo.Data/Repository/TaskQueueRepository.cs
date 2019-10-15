//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;

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