using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
 public interface   IBinaryFile: IExtensionable
    {
          byte[] ContentBytes { get; set; }

          int Size { get; set; } 

    }
}
