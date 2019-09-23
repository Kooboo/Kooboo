using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.Cache
{
    public class SiteCache
    {
        public long LastContentLog { get; set; }

        public long LastKDatabaseLog { get; set; }
         

        public Dictionary<Guid, PageCache> Pages { get; set; } 
    }
}
