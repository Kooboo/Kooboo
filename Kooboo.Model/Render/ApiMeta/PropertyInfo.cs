using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render.ApiMeta
{
    public class PropertyInfo
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public List<ValidationRules.ValidationRule> Rules { get; set; }
    }
}
