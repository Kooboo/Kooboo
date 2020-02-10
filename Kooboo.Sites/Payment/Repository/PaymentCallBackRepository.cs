using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Repository
{  
    public class PaymentCallBackRepository : Kooboo.Sites.Repository.SiteRepositoryBase<PaymentCallback>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters(); 
                para.AddColumn<PaymentCallback>(o => o.RequestId);
                para.SetPrimaryKeyField<PaymentCallback>(o => o.Id);
                return para;
            }
        } 
    }
}
