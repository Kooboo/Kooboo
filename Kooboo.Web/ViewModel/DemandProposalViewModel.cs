using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class DemandProposalViewModel
    {
        public string Description { get; set; }

        public string UserName { get; set; }

        public string Budget { get; set; }

        public bool WinTheBidding { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// days
        /// </summary>
        public int Duration { get; set; }
    }
}
