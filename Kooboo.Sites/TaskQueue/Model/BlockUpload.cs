//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.TaskQueue.Model
{
    public class BlockUpload
    {
        public string PackageId { get; set; }
        
        public string LocalPath { get; set; }

        public string Hash { get; set; }
    }
}
