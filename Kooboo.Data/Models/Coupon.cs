using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
   
    public class Coupon : IGolbalObject
    {
        private Guid _id; 
        public Guid Id {
            get
            {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrWhiteSpace(this.Code))
                    {
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Code); 
                    }
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }
        }

        public decimal Amount { get; set; }

        // Only for the organization of this country...
        public string Country { get; set; }

        public bool IsUsed { get; set; }

        public Guid OrganizationId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Code { get; set; }
     
    }
}
