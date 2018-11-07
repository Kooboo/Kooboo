using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class Demand : IGolbalObject
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

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public DemandStatus Status { get; set; }

        public Guid WinProposalId { get; set; }//中标id

        public string Skills { get; set; }

        public decimal Budget { get; set; }

        public int ProposalCount { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public enum DemandStatus
    {
        Tendering=0,
        EndOfTender=1,
        UnFinished=2,
        Finish=3,
        Invalid=4
    }
}
