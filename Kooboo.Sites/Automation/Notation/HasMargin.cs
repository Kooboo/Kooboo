using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation.Notation
{
   public class HasMargin : INotation
    {
        public string Name
        {
            get { return "HasMargin"; }
        }

        public Type ReturnType
        {
            get { return typeof(bool); }
        }

        public object Execute(Dom.Element element)
        {
            return element.RawComputedStyle.hasPartialProperty("margin");
        }

        public List<string> ReturnStringValueList
        {
            get {return null; }
        }
    }
}
