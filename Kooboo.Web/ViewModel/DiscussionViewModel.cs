using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class DiscussionViewModel
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }

        public string UserName { get; set; }

        public DateTime LastModified { get; set; }

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }
    }
}
