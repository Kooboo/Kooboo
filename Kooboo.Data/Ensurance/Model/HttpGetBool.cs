using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance.Model
{

    // this method must return bool.
  public  class HttpGetBool : IQueueTask
    {
        public string FullUrl { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
