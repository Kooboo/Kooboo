using Kooboo.IndexedDB;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Repository; 

namespace Kooboo.Sites.Ecommerce.Repository
{
  public   class OrderRepository : SiteRepositoryBase<Order>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddIndex<Order>(o => o.CustomerId);
                para.AddColumn<Order>(o => o.IsPaid);
                return para;
            }
        } 
    }
}
