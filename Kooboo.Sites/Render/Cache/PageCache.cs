using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.PageCache
{
    public static class PageCache
    {
        private static object _locker = new object();

        internal static Dictionary<Guid, Dictionary<Guid, PageCacheItem>> SitePageCache { get; set; }

        static PageCache()
        {
            SitePageCache = new Dictionary<Guid, Dictionary<Guid, PageCacheItem>>();
        }

        internal static Dictionary<Guid, PageCacheItem> GetSiteCaches(Guid SiteId)
        {
            if (!SitePageCache.ContainsKey(SiteId))
            {
                lock (_locker)
                {
                    Dictionary<Guid, PageCacheItem> sitecache = new Dictionary<Guid, PageCacheItem>();
                    SitePageCache.Add(SiteId, sitecache);
                }
            }
            return SitePageCache[SiteId];

        }
         
        public static string GetByVersion(Guid SiteId, Guid PageId, long Version, Dictionary<string, string> querystring = null)
        {
            var hashkey = PageCacheItem.ComputeHash(querystring); 

            var siteitems = GetSiteCaches(SiteId);
            if (siteitems.ContainsKey(PageId))
            {
                var item = siteitems[PageId];
                if (item != null && item.QueryStringHash == hashkey)
                {
                    // by version. 
                    if (Version == item.Version)
                    {
                        return item.Result;
                    } 
                }
            } 
            return null;
        }
         
        public static string GetByMinutes(Guid SiteId, Guid PageId, int CacheMinutes, Dictionary<string,string> querystring,  long version=-1)
        {
            var hashkey = PageCacheItem.ComputeHash(querystring);
            var siteitems = GetSiteCaches(SiteId);
            if (siteitems.ContainsKey(PageId))
            {
                var item = siteitems[PageId];
                if (item != null && item.QueryStringHash == hashkey)
                {
                    // force update when page version change. 
                    if (version >0 && item.Version != version)
                    {
                        return null; 
                    } 
                    // by minutes
                    var MinutesPast = DateTime.Now - item.LastModify;
                    if (MinutesPast.Minutes < CacheMinutes)
                    {
                        return item.Result;
                    }
                }
            } 
            return null;
        }
         
        public static void Set(Guid SiteId, Guid PageId, string Content, long Version, Dictionary<string, string> querystring)
        {
            var siteitems = GetSiteCaches(SiteId);

            var cacheitem = new PageCacheItem();
            cacheitem.Result = Content;
            cacheitem.LastModify = DateTime.Now;
            cacheitem.ObjectId = PageId;
            cacheitem.Version = Version;
            cacheitem.QueryString = querystring; 
            siteitems[PageId] = cacheitem;

        }
    }
}
