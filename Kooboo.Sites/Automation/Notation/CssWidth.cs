using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Extensions;

namespace Kooboo.Sites.Automation.Notation
{
 public   class CssWidth : INotation
    {
     
        public string Name
        {
            get { return "CssWidth"; }
        }

        public Type ReturnType
        {
            get { return typeof(int); }
        }

        public object Execute(Dom.Element element)
        {
            string widthvalue = element.RawComputedStyle.getPropertyValue("width");

            int value = widthvalue.GetDigitPart();

            if (widthvalue.Contains("%"))
            {
                value = 1024 * value;     // make the default screen resolution = 1024 now. 
            }

            return value; 
        }

        public List<string> ReturnStringValueList
        {
            get { return null; }
        }
    }
}
