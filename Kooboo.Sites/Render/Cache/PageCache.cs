using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.PageCache
{
    public static class PageCache
    {
        private static object _locker = new object();

        internal static Dictionary<Guid, Dictionary<Guid, CacheItem>> SitePageCache { get; set; }
        static PageCache()
        {
            SitePageCache = new Dictionary<Guid, Dictionary<Guid, CacheItem>>();
        }

        internal static Dictionary<Guid, CacheItem> GetSiteCaches(Guid SiteId)
        {
            if (!SitePageCache.ContainsKey(SiteId))
            {
                lock (_locker)
                {
                    Dictionary<Guid, CacheItem> sitecache = new Dictionary<Guid, CacheItem>();
                    SitePageCache.Add(SiteId, sitecache);
                }
            }
            return SitePageCache[SiteId];

        }
         
        public static string GetByVersion(Guid SiteId, Guid PageId, long Version)
        {
            var siteitems = GetSiteCaches(SiteId);
            if (siteitems.ContainsKey(PageId))
            {
                var item = siteitems[PageId];
                if (item != null)
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


        public static string GetByMinutes(Guid SiteId, Guid PageId, int CacheMinutes, long version=-1)
        {
            var siteitems = GetSiteCaches(SiteId);
            if (siteitems.ContainsKey(PageId))
            {
                var item = siteitems[PageId];
                if (item != null)
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


        public static void Set(Guid SiteId, Guid PageId, string Content, long Version)
        {
            var siteitems = GetSiteCaches(SiteId);

            var cacheitem = new CacheItem();
            cacheitem.Result = Content;
            cacheitem.LastModify = DateTime.Now;
            cacheitem.ObjectId = PageId;
            cacheitem.Version = Version;

            siteitems[PageId] = cacheitem;

        }
    }
}
