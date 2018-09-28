using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Models
{
  public  class Attachment
    {
        public string FileName { get; set; }
         
        public string Type { get; set; }

        public string SubType { get; set; }

        public long Size { get; set; }
    }
}
