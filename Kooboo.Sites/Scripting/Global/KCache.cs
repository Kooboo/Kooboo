using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Scripting
{
  public  class KCache
    { 

        [KIgnore]
        private RenderContext context { get; set; }

        public KCache(RenderContext context)
        {
            this.context = context;
        }

        [Description("Cache external resource into local server")]
        public string LocalCache(string externalUrl, int hours)
        {
            var itemPath =  Kooboo.Sites.Render.LocalCache.LocalCacheManager.SetItem(this.context.WebSite, externalUrl, hours); 
            return Render.LocalCache.LocalCacheManager.GetUrl(itemPath);
        } 
    }
}
 