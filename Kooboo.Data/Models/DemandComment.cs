using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class DemandComment:IGolbalObject
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

        public Guid ProposalId { get; set; }

        public Guid ParentCommentId { get; set; }

        public int CommentCount { get; set; }

        public string Content { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        //public bool IsShare { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
