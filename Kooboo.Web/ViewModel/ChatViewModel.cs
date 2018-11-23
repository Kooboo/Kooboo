using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Models;

namespace Kooboo.Web.ViewModel
{
    public class ChatViewModel
    {
        public Guid Id { get; set;}

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public ChatType ChatType { get; set; }

        public string Content { get; set; }

        public List<ResouceAttachment> Attachments { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
