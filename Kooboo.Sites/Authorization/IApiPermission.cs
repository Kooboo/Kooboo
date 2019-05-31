using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Authorization
{
   public interface IApiPermission
    { 
        // linked permission tree. 
        // developement/view
        string PermissionTree { get; }
    }
}
