//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Authorization.Model;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Cache
{
    public class WebSiteCache
    {
        private static object _locker = new object();

        public static void RemoveWebSitePlan(Guid webSiteId)
        {
            string id = webSiteId.ToString();

            //lock (_locker)
            //{
            //    var pagekeys = PageRenderPlan.Where(o => o.Key.StartsWith(id)).Select(o => o.Key).ToList();

            //    //foreach (var item in PageRenderPlan.Where(o => o.Key.StartsWith(id)))
            //    //{
            //    //    pagekeys.Add(item.Key);
            //    //}
            //    foreach (var item in pagekeys)
            //    {
            //        PageRenderPlan.Remove(item);
            //    }

            //    var layoutkeys = LayoutRenderPlan.Where(o => o.Key.StartsWith(id)).Select(o => o.Key).ToList();
            //    //foreach (var item in LayoutRenderPlan.Where(o => o.Key.StartsWith(id)))
            //    //{
            //    //    LayoutRenderPlan.Remove(item.Key);
            //    //}
            //    foreach (var item in layoutkeys)
            //    {
            //        LayoutRenderPlan.Remove(item);
            //    }

            //    var viewkeys = ViewRenderPlan.Where(o => o.Key.StartsWith(id)).Select(o => o.Key).ToList();
            //    //foreach (var item in ViewRenderPlan.Where(o => o.Key.StartsWith(id)))
            //    //{
            //    //    ViewRenderPlan.Remove(item.Key);
            //    //}
            //    foreach (var item in viewkeys)
            //    {
            //        ViewRenderPlan.Remove(item);
            //    }
            //}
        }

        private static object _object = new object();

        private static Dictionary<Guid, SiteDb> _siteDbs = new Dictionary<Guid, SiteDb>();

        public static SiteDb GetSiteDb(WebSite website)
        {
            if (website == null)
            {
                return null;
            }
            if (!_siteDbs.ContainsKey(website.Id))
            {
                lock (_object)
                {
                    if (!_siteDbs.ContainsKey(website.Id))
                    {
                        var sitedb = new SiteDb(website);
                        _siteDbs[website.Id] = sitedb;
                    }
                }
            }
            var result = _siteDbs[website.Id];
            result.WebSite = website;
            return result;
        }

        public static void SetNull(Guid siteId)
        {
            _siteDbs[siteId] = null;
        }

        public static void RemoveNull(Guid siteId)
        {
            _siteDbs.Remove(siteId);
        }

        public static void Remove(SiteDb sitedb)
        {
            Cache.RenderPlan.RemoveSiteDb(sitedb.Id);
            Cache.RouteTreeCache.RemoveSiteDb(sitedb.Id);

            Cache.SiteObjectCache<Label>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<View>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<Layout>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<ViewDataMethod>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<DataMethodSetting>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<SyncSetting>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<ContentFolder>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<ContentType>.RemoveSiteDb(sitedb.Id);

            Cache.SiteObjectCache<ContentCategory>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<SiteCluster>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<KConfig>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<Code>.RemoveSiteDb(sitedb.Id);

            //    || TValueType == typeof(ContentCategory)
            //    || TValueType == typeof(SiteCluster)
            //    || TValueType == typeof(Code)
            //    || TValueType == typeof(KConfig)

            Cache.SiteObjectCache<Page>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<Route>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<HtmlBlock>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<Style>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<Script>.RemoveSiteDb(sitedb.Id);
            Cache.SiteObjectCache<Image>.RemoveSiteDb(sitedb.Id);

            _siteDbs.Remove(sitedb.WebSite.Id);
        }

        public static bool EnableCache(WebSite website, Type valueType)
        {
            if (valueType == typeof(Label)
                || valueType == typeof(View)
                || valueType == typeof(Layout)
                || valueType == typeof(ViewDataMethod)
                || valueType == typeof(DataMethodSetting)
                || valueType == typeof(SyncSetting)
                || valueType == typeof(ContentFolder)
                || valueType == typeof(ContentType)
                || valueType == typeof(Route)
                || valueType == typeof(ContentCategory)
                || valueType == typeof(SiteCluster)
                || valueType == typeof(Code)
                || valueType == typeof(KConfig)
                || valueType == typeof(TableRelation)
                || valueType == typeof(RolePermission)
                || valueType == typeof(SiteUser)
                )
            {
                return true;
            }

            if (!Kooboo.Data.AppSettings.Global.IsOnlineServer)
            {
                if (valueType == typeof(Page)
              || valueType == typeof(HtmlBlock)
              || valueType == typeof(Style)
              || valueType == typeof(Script)
              || valueType == typeof(Image)
               )
                {
                    return true;
                }
            }

            return false;
        }
    }
}