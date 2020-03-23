using Kooboo.Data;
using Kooboo.Data.Attributes;
using Kooboo.Data.Interface;
using Kooboo.IndexedDB.CustomAttributes;
using Kooboo.Sites.Payment.Callback;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Payment
{
    [Serializable]
    public class PaymentCallback : IGolbalObject, ISiteObject
    {
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                    _id = Lib.Helper.IDHelper.NewTimeGuid(DateTime.UtcNow);
                return _id;
            }
            set { _id = value; }
        }
        public Guid RequestId { get; set; }

        public string ResponseMessage { get; set; }

        public string RawData { get; set; }

        [Description("The call back relative url includes query string")]
        public string RequestUrl { get; set; }

        [Description("Form post data")]
        public string PostData { get; set; }

        [Description("Orginal request data")]
        [KoobooIgnore]
        public KScript.KDictionary Request { get; set; }

        // when reponse with paid, set this to true and update the related order. 
        public bool Paid
        {
            get
            {
                return Status == PaymentStatus.Paid || this.Status == PaymentStatus.Authorized;
            }
        }

        public bool Cancelled
        {
            get
            {
                return Status == PaymentStatus.Cancelled;
            }
        }

        public bool Rejected
        {
            get
            {
                return Status == PaymentStatus.Rejected;
            }
        }

        public bool Pending
        {
            get
            {
                return Status == PaymentStatus.Pending;
            }
        }

        // Pending — your payment has not been sent to the bank or credit card processor.
        public PaymentStatus Status { get; set; }
         
        // response to payment provider. 
        [KoobooIgnore]
        [KIgnore]
        public CallbackResponse CallbackResponse
        {
            get;set;
        }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [KIgnore]
        public byte ConstType { get; set; } = ConstObjectType.PaymentCallback;

        public DateTime LastModified { get; set; }

        public string Name { get; set; }
    }



}
