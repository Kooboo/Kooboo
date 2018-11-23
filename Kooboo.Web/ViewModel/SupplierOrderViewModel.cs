using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class SupplierOrderViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public Guid SupplierId { get; set; }

        public string Expertise { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }

        public Dictionary<string,string> Status { get; set; }

    }

}
