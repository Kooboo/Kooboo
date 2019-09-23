using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.Cache
{
  public  class PageCache
    {

        public Guid PageId { get; set; }

        public List<string> BindingFolders { get; set; }

        public List<string> BindingTables { get; set; }
         
        // GUID = Hash or raw url. 
        public Dictionary<Guid, string> Contents { get; set; }

    }
}
