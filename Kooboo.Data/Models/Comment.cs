using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class Comment : IGolbalObject
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

        public Guid ParentCommentId { get; set; } = Guid.Empty;

        public Guid DiscussionId { get; set; }

        public string Content { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public int CommentCount { get; set; }

        public DateTime LastModified { get; set; }
    }
}
