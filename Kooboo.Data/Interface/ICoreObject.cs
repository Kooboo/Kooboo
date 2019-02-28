//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{ 
    /// <summary>
    /// Core object are essential objects for the system... 
    /// Those objects should be synchronize to remote servers, enable log and version.  
    /// </summary> 
  public  interface ICoreObject
    { 
        bool Online { get; set; }

        long Version { get; set; }
    }
}
