using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.ViewModel;

namespace Kooboo.Data.Models
{
    public class PayPackageRequest
    {
        public Guid TemplateId { get; set; }
        public Guid BuyUserId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
