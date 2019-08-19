using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance.Model
{
   public class HttpPost : IQueueTask
    {
        public string FullUrl { get; set; }

        public string Json { get; set; }
    }
}
