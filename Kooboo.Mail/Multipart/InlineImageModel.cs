using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Multipart
{
    public class InlineImageModel
    {
        public string FileName { get; set; }

        public byte[] Binary { get; set; }

        public string ContentId { get; set; } 
    }
}
