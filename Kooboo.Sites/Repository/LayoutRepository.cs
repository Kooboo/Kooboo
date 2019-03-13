//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Repository
{
  public  class LayoutRepository : SiteRepositoryBase<Layout>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddColumn<Layout>(o => o.Id);
                para.AddColumn<Layout>(o => o.Name);
                para.SetPrimaryKeyField<Layout>(o => o.Id);
                return base.StoreParameters;
            }
        }
    }
}
