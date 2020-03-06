using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Repository
{ 
    public class PaymentRequestRepository : Kooboo.Sites.Repository.SiteRepositoryBase<PaymentRequest>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddIndex<PaymentRequest>(o => o.OrderId);
                para.AddColumn<PaymentRequest>(o => o.Paid);
                para.AddColumn<PaymentRequest>(o => o.Failed);
                para.AddColumn<PaymentRequest>(o => o.ReferenceIdHash);
                return para;
            }
        }
         
        public bool UpdatePaid(Guid ReqeustId)
        {
            var reqeust = this.Get(ReqeustId);
            if (reqeust != null)
            {
                this.Store.UpdateColumn<bool>(ReqeustId, o => o.Paid, true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateCancel(Guid ReqeustId)
        {
            var reqeust = this.Get(ReqeustId);
            if (reqeust != null)
            {
                this.Store.UpdateColumn<bool>(ReqeustId, o => o.Failed, true);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}