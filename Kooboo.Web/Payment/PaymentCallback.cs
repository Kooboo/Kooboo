using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.IndexedDB.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.ServerData.Model
{
    public class PaymentCallback : IGolbalObject, ISiteObject
    {
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                    _id = Guid.NewGuid();
                return _id;
            }
            set { _id = value; }
        }
        public Guid PaymentRequestId { get; set; }

        // all kinds of response. save them all. 
        public string Response { get; set; } 

        public string RawData { get; set; }

        // when reponse with paid, set this to true and update the related order. 
        public bool IsPaid { get; set; }

        public bool IsCancel { get; set;  } 
        
        // response to payment provider. 
        [KoobooIgnore]
        public Kooboo.Api.ApiResponse.IResponse CallbackResponse { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public byte ConstType { get; set; } = ConstObjectType.PaymentCallback; 

        public DateTime LastModified { get; set; }

        public string Name { get; set; }
    }
}
