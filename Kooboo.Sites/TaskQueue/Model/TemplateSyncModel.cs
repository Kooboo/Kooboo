//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Sites.TaskQueue.Model
{
  public  class TemplateSyncModel
    {
        public   Guid PackageId { get; set; } 

        public bool IsDelete { get; set; }

        public bool HasBinaryChange { get; set; } = true; 
    }
}
