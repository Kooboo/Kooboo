using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Models;

namespace Kooboo.Web.ViewModel
{
    public class SupplierItemViewModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public bool IsOwner { get; set; }

        public string Introduction { get; set; }

        public List<SupplierExpertise> Expertises { get; set; }

        public string Currency { get; set; }

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }

        public List<ResouceAttachment> Attachments { get; set; }
    }
}
