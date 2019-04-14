using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.ValidationRules
{
    public class MinLengthRule:ValidationRule
    {
        public int MinLength;

        public MinLengthRule(int minLength, string message)
        {
            MinLength = minLength;
            Message = string.Format(message, minLength).Replace("\"", "\\\"");
        }

        public override string GetRule()
        {
            return string.Format("{{type:\"minLength\",minLength:{1},message:\"{0}\"}}", Message, MinLength);
        }
    }
}
