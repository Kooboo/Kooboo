using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class DemandViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string UserName { get; set; }

        public DateTime CreateTime { get; set; }

        public decimal Budget { get; set; }

        public string Currency { get; set; }

        public string Symbol { get; set; }

        public int ProposalCount { get; set; }

        public List<string> Skills { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
