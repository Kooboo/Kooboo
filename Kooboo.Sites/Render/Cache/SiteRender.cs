using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Render;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Cache
{
    public static class SiteRender
    { 
        //public static Dictionary<Guid, SiteCache> SiteCaches { get; set; } = new Dictionary<Guid, SiteCache>();  
       

        //public static async Task<string> RenderPageWithCache(FrontContext context, Page page, Func<FrontContext, Task<string>> RenderFunc)
        //{

        //    if (context.WebSite.EnableCache)
        //    {
        //        var sitecache = GetSiteCache(context.WebSite);
                  
        //        var guid = Lib.Security.Hash.ComputeGuidIgnoreCase(context.RenderContext.Request.RawRelativeUrl);

        //        if (sitecache.Pages.ContainsKey(guid))
        //        {
        //            return sitecache.Pages[guid]; 
        //        }
        //        else
        //        {
                   
        //        } 
        //    }
        //    else

        //    {
        //        return await RenderFunc(context);
        //    }
            
        //    if (page.CacheType == SiteRenderCacheType.ContentLink)
        //    {  
        //        var lastkey = context.SiteDb.DatabaseDb.Log.Store.LastKey; 
        //    }

            
        //}
        
        //public static void ResetSite(Guid SiteId)
        //{
        //    SiteCaches[SiteId] = new SiteCache(); 
        //}
         
        //private static SetPageCache(WebSite site, Page page, string RawUrl, string content)
        //{
        //    var sitecache = GetSiteCache(site);
        //    sitecache.Pages[UrlHash] = content; 
        //}

        //private static SiteCache GetSiteCache(WebSite Site)
        //{
        //    if (!SiteCaches.ContainsKey(Site.Id))
        //    { 
        //        SiteCaches[Site.Id] = new SiteCache() { LastContentLog = Site.SiteDb().Log.Store.LastKey }; 
        //    }
        //    else
        //    {
        //        var currentLastLog = Site.SiteDb().Log.Store.LastKey;
        //        var currentSiteCache = SiteCaches[Site.Id]; 
                
        //        if (currentSiteCache.LastContentLog == currentLastLog)
        //        {
        //            return currentSiteCache; 
        //        }
        //    }
        //    return SiteCaches[Site.Id]; 
        //}
    } 
     
}
