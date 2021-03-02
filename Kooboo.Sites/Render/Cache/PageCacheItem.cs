using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; 

namespace Kooboo.Sites.Render.PageCache
{
  public   class PageCacheItem
    { 
        public Guid ObjectId { get; set; }

        public long Version { get; set; }

        public string Result { get; set; }

        public DateTime LastModify { get; set; }

        private Guid _QueryStringHash { get; set; } 

        public Guid QueryStringHash
        {
          get;
          set; 
        }

        private Guid ComputeHash(Dictionary<string, string> value)
        {
            if (value == null)
            { return default(Guid); } 

            string uniquestring = string.Empty; 
            var ordered = value.OrderBy(o => o.Key);
            foreach (var item in ordered)
            {
                uniquestring += item.Key + item.Value; 
            } 
            return Kooboo.Lib.Security.Hash.ComputeGuidIgnoreCase(uniquestring);  
        }

        private Dictionary<string, string> _querystring; 
        public Dictionary<string, string> QueryString {
            get
            {
                return _querystring; 
            }
            set
            {
                _querystring = value;
                this.QueryStringHash = ComputeHash(value);  
            } 
        }

    }
}
