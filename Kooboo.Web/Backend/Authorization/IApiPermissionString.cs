using Kooboo.Web.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Authorization
{
   public interface IApiPermissionString : IPermissionControl
    { 
        // linked permission tree. 
        // developement/view
        string Permission { get; }
    }
}
