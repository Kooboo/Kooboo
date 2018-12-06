using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class UserExpertiseViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }
    }

    public class UserExpertiseItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }
    }
}
