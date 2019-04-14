using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.ValidationRules
{
    public class BetweenRule:ValidationRule
    {
        public int From;
        public int To;
        public BetweenRule(int from ,int to,string message)
        {
            From = from;
            To = to;
            Message = string.Format(message, From, To).Replace("\"", "\\\"");
        }

        public override string GetRule()
        {
            return string.Format("{{type:\"between\",from:{0},to:{1},message:\"{2}\"}}",From,To ,Message);
        }
    }
}
