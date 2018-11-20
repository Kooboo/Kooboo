using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Kooboo.Data.Models;
using Newtonsoft.Json.Linq;

namespace Kooboo.Web.ViewModel
{
    public class DemandItemViewModel
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

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsOwner { get; set; }

        public string UserName { get; set; }

        public Dictionary<string, string> Status { get; set; }

        public List<string> Skills { get; set; }

        public decimal Budget { get; set; }

        public int ProposalCount { get; set; }

        public List<DemandAttachment> Attachments { get; set; }

        public string Currency { get; set; }

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }

        public DateTime CreateTime { get; set; }

        public DateTime LastModify { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
