using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render.ApiMeta
{
    public class ModelInfo
    {
        public Type Type { get; set; }

        public List<PropertyInfo> Properties { get; set; }
    }
}
