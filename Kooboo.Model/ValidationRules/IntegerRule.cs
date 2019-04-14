using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.ValidationRules
{
    public class IntegerRule:ValidationRule
    {
        public IntegerRule(string message)
        {
            Message = message;
        }

        public override string GetRule()
        {
            return string.Format("{{type:\"integer\",message:\"{0}\"}}", Message);
        }
    }
}
