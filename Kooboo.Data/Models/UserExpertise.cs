using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class UserExpertise: IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public List<ResouceAttachment> Attachments { get; set; }
    }
}
