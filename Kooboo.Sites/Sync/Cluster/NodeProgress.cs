//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync.Cluster
{
   public class NodeProgress
    {
        public Guid ServerId { get; set; }

        public long CurrentVersion { get; set; } = -1; 

        public DateTime LastUpdate { get; set; }
    }
}
