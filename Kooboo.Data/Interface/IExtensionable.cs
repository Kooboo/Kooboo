using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
   public interface IExtensionable : ISiteObject
    {
        string Extension { get; set; }
    }
}
