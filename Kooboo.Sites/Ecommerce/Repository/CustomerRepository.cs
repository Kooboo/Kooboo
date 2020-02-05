using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Repository
{ 
    public class CustomerRepository : SiteRepositoryBase<Models.Customer>
    { 
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddIndex<Models.Customer>(o => o.EmailId);
                para.AddIndex<Models.Customer>(o => o.TelHash);
                return para;
            }
        }
       
    }
}
