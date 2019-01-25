//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.TaskQueue.Model
{
  public  class WriteToDisk
    {
        public bool IsDelete { get; set; }

        public bool IsBinary { get; set; }
        public byte[] BinaryBytes { get; set; }

        public string TextBody { get; set; } 

        public string FullPath { get; set; }

        public  Guid ObjectId { get; set; } 
    }
}
