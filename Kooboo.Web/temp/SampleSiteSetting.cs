using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.temp
{
    public class SampleSiteSetting : Kooboo.Data.Interface.ISiteSetting
    {
        public string Name => "samplename";

        public string fieldone { get; set; }

        public string Fieldtwo { get; set; }

        public int counter { get; set; }
    }
}
