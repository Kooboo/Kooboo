using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Repository
{ 
    public class CartRepository : SiteRepositoryBase<Cart>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters(); 
                para.AddIndex<Cart>(o => o.CustomerId);
                para.SetPrimaryKeyField<Cart>(o => o.Id); 
                return para;
            }
        } 
    }
}
