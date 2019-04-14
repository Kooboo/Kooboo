using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.ValidationRules
{
    public class NumericRule:ValidationRule
    {
        public NumericRule(string message)
        {
            Message = message;
        }

        public override string GetRule()
        {
            return string.Format("{{type:\"numeric\",message:\"{0}\"}}",Message);
        }

    }
}
