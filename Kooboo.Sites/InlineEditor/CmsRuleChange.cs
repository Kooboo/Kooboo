using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor
{
  public  class CmsRuleChange
    {
        public Guid CmsRuleId { get; set; }

        public string PropertyName { get; set; }

        public string Value { get; set; }

        public bool Important { get; set; }
    }
}
