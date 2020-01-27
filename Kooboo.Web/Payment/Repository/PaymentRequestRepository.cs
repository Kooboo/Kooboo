using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Payment.Repository
{ 
    public class TempPaymentRequestRepository : Kooboo.Sites.Repository.SiteRepositoryBase<PaymentRequest>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddIndex<PaymentRequest>(o => o.OrderId);
                para.AddColumn<PaymentRequest>(o => o.IsPaid);
                para.AddColumn<PaymentRequest>(o => o.IsCancel);
                return para;
            }
        }
         
        public bool UpdatePaid(Guid ReqeustId)
        {
            var reqeust = this.Get(ReqeustId);
            if (reqeust != null)
            {
                this.Store.UpdateColumn<bool>(ReqeustId, o => o.IsPaid, true);
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
                this.Store.UpdateColumn<bool>(ReqeustId, o => o.IsCancel, true);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}