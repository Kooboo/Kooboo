using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor.Model

{
   public class HtmlblockModel : IInlineModel
    {
        public ActionType Action
        {
            get;set;
        }

        public string EditorType
        {
            get
            {
                return "htmlblock"; 
            }
        }

        public string NameOrId
        {get;set;
        }

        public string ObjectType
        {
            get;set;
        }

        public string Value
        {
            get;set;
        }
    }
}
