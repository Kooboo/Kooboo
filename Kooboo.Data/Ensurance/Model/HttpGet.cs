using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance.Model
{
  public  class HttpGet : IQueueTask
    {
        public string FullUrl { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
