using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Payment
{
    public class PaymentStatusResponse
    {
        public bool HasResult { get; set; }

        private bool SetPaid;
        private bool SetFailed;

        private bool _paid;
        public bool Paid
        {
            get
            {
                if (SetPaid)
                {
                    return _paid;
                }
                else

                {
                    if (this.Status == PaymentStatus.Authorized || this.Status == PaymentStatus.Paid)
                    {
                        return true;
                    } 
                    else 
                    {
                        return false;
                    } 
                }

            }
            set
            {
                _paid = value;
                SetPaid = true;
            }
        }

        public bool Failed { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus Status
        {
            get; set;
        }

        public string Message { get; set; }
    }
}
