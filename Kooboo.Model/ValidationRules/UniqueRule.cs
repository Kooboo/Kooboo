using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.ValidationRules
{
    public class UniqueRule : ValidationRule
    {
        public string Api { get; set; }
        public UniqueRule(string api,string message)
        {
            Api = api;
            Message = message;
        }

        public override string GetRule()
        {
            return string.Format("{{type:\"unique\",api:{1},message:\"{0}\"}}", Message,Api);
        }
    }
}
