using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.ValidateRules
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequireFieldRule:Rule
    {
        public string FieldName { get; set; }
        public string Trigger { get; set; } = "blur";
        public RequireFieldRule(string fieldName,string message)
        {
            FieldName = fieldName;
            Message = string.Format(message,fieldName);
        }

        public override string GetRule()
        {
            return string.Format("{{type:required,message:'{0}',trigger:'{1}'}}", Message, Trigger);
        }
    }
}
