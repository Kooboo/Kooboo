using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Interface
{
   public interface IMailServerProvider
    {
        bool IsLocal(Organization org);  
    }
}
