using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class Chat:IGolbalObject
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

        public Guid RelativeId { get; set; }

        //public Guid RelativeUserId { get; set; }

        //public string RelativeUserName { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public ChatType ChatType { get; set; }

        public string Content { get; set; }

        public List<ResouceAttachment> Attachments { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public enum ChatType
    {
        DemandProposal=0,
        Supply=1
    }
}
