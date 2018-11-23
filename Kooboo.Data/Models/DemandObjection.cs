using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class DemandObjection:IGolbalObject
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

        public Guid DemandId { get; set; }

        public string DemandTitle { get; set; }

        public Guid DemandUserId { get; set; }

        public string DemandUserName { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

        public string Contact { get; set; }

        public DateTime CreateTime { get; set; }

        public DemandObjectionStatus Status { get; set; }

        public string Remark { get; set; }

    }

    public enum DemandObjectionStatus
    {
        UnResolved=0,
        Resolved=1
    }
}
