//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Events.Cms;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Models;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Authorization.Model;

namespace Kooboo.Sites.Cache
{
    public class WebSiteCache
    {
        private static object _locker = new object();

        public static void RemoveWebSitePlan(Guid WebSiteId)
        {
            string id = WebSiteId.ToString();

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

        private static Dictionary<Guid, SiteDb> _SiteDbs = new Dictionary<Guid, SiteDb>();

        public static SiteDb GetSiteDb(WebSite website)
        {
            if (website == null)
            {
                return null;
            }
            if (!_SiteDbs.ContainsKey(website.Id))
            {
                lock (_object)
                {
                    if (!_SiteDbs.ContainsKey(website.Id))
                    {
                        var sitedb = new SiteDb(website);
                        _SiteDbs[website.Id] = sitedb;
                    }
                }
            }
            var result = _SiteDbs[website.Id];
            result.WebSite = website;
            return result;
        }

        public static void SetNull(Guid SiteId)
        {
            _SiteDbs[SiteId] = null;   
        }

        public static void RemoveNull(Guid SiteId)
        {
            _SiteDbs.Remove(SiteId); 
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
             

            _SiteDbs.Remove(sitedb.WebSite.Id);
        }


        public static bool EnableCache(WebSite website, Type TValueType)
        {
            if (TValueType == typeof(Label)
                || TValueType == typeof(View)
                || TValueType == typeof(Layout)
                || TValueType == typeof(ViewDataMethod)
                || TValueType == typeof(DataMethodSetting)
                || TValueType == typeof(SyncSetting)
                || TValueType == typeof(ContentFolder)
                || TValueType == typeof(ContentType)
                || TValueType == typeof(Route)
                || TValueType == typeof(ContentCategory)
                || TValueType == typeof(SiteCluster)
                || TValueType == typeof(Code)
                || TValueType == typeof(KConfig)
                || TValueType == typeof(TableRelation)
                || TValueType == typeof(RolePermission)
                || TValueType == typeof(SiteUser)
                )
            {
                return true;
            }

            if (!Kooboo.Data.AppSettings.Global.IsOnlineServer)
            {
                if (TValueType == typeof(Page)
              || TValueType == typeof(HtmlBlock)
              || TValueType == typeof(Style)
              || TValueType == typeof(Script)
              || TValueType == typeof(Image)
               )
                {
                    return true;
                }
            }

            return false;
        }

    }
}
