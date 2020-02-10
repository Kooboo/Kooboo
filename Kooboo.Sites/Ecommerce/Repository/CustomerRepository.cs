using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.Ecommerce.Repository
{ 
    public class CustomerRepository : SiteRepositoryBase<Models.Customer>
    { 
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddIndex<Customer>(o => o.EmailHash);
                para.AddIndex<Customer>(o => o.TelHash);
                return para;
            }
        }
    }
}
