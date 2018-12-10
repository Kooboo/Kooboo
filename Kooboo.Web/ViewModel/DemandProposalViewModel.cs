using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Models;

namespace Kooboo.Web.ViewModel
{
    public class DemandProposalViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string Budget { get; set; }

        public string Currency { get; set; }
        
        public string Symbol { get; set; }

        public bool WinTheBidding { get; set; }

        public string DemandTitle { get; set; }

        public Guid DemandId { get; set; }

        public string ProposalType { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastModify { get; set; }

        /// <summary>
        /// days
        /// </summary>
        public int Duration { get; set; }
    }
}
