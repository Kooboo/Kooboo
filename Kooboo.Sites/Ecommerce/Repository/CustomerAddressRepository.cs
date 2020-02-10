using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Repository
{
   public class CustomerAddressRepository : SiteRepositoryBase<CustomerAddress>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddIndex<CustomerAddress>(o => o.CustomerId);
                return para;
            }
        } 
    }
}
