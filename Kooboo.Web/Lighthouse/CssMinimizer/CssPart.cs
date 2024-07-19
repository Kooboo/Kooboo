using System.Collections.Generic;

namespace Kooboo.Web.Lighthouse.CssMinimizer
{
    public class CssPart
    {
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }   
        public string ConditionText { get; set; }
         
        public List<string> Media { get; set; } = new List<string>();

    }
}
