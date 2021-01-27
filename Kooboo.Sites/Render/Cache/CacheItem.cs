using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.PageCache
{
  public   class CacheItem
    { 
        public Guid ObjectId { get; set; }

        public long Version { get; set; }

        public string Result { get; set; }

        public DateTime LastModify { get; set; }

    }
}
