using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor
{
    public class InlineSourceUpdate
    {
        public string KoobooId { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set;  }

        public bool IsDelete { get; set; }
    }
}
