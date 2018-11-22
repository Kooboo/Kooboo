using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class Supplier: IGolbalObject
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

        public Guid UserId { get; set; }
        
        public string UserName { get; set; }
       
        public string Introduction { get; set; }

        public List<SupplierExpertise> Expertises { get; set; }

        public string Currency { get; set; }

    }

    public class SupplierExpertise
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
