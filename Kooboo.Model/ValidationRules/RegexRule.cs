using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.ValidationRules
{
    public class RegexRule:ValidationRule
    {
        public string Regex { get; set; }
        public RegexRule(string regex,string message)
        {
            Regex = regex;
            Message = message;
        }

        public override string GetRule()
        {
            return string.Format("{{type:\"regex\",regex:\"{0}\",message:\"{1}\"}}",Regex, Message);
        }
    }
}
