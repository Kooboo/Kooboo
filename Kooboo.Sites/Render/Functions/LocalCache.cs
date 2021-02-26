using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using System.Linq; 

namespace Kooboo.Sites.Render.Functions
{
    //cache external resource as local.
    public class LocalCache : IFunction
    {
        public string Name => "LocalCache";

        public List<IFunction> Parameters { get; set; }

        public object Render(RenderContext context)
        {
            var paras = FunctionHelper.RenderParameter(context, this.Parameters);
            if (paras.Any())
            {
                var item = paras[0]; 
                if (item !=null)
                {
                    string fullurl = item.ToString().ToLower();

                    if (fullurl.StartsWith("https://") || fullurl.StartsWith("http://") || fullurl.StartsWith("//"))
                    {
                        var uri = new Uri(fullurl); 
                        if (uri.Host != context.Request.Host)
                        {
                            // do the cache.  
                            var cacheitem = Kooboo.Sites.Render.LocalCache.LocalCacheManager.SetItem(context.WebSite, fullurl);
                            return Sites.Render.LocalCache.LocalCacheManager.GetUrl(cacheitem);  
                        } 
                    } 

                }  
            } 
            return null; 
        }
    }  
}
