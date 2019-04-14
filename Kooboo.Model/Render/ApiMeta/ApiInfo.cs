using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render.ApiMeta
{
    public class ApiInfo
    {
        public ModelInfo Result { get; set; }

        public List<PropertyInfo> Parameters { get; set; }

        public ModelInfo Model { get; set; }
    }
}
