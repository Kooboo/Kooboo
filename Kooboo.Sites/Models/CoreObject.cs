//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Interface; 

namespace Kooboo.Sites.Models
{ 
    /// <summary>
    /// Core object are essential objects for the system... 
    /// Those objects should be synchronize to remote servers, enable log and version.  
    /// </summary> 
  public  class CoreObject : SiteObject, ICoreObject
    {
        public bool Online { get; set; } = true; 

        [Kooboo.Attributes.SummaryIgnore]
        public Int64 Version { get; set; } 
    }
}
