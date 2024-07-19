//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Api.Implementation
{
    public class SearchApi : IApi
    {
        public string ModelName => "Search";
        public bool RequireSite => true;
        public bool RequireUser => true;

        [Kooboo.Attributes.RequireParameters("rebuild")]
        [Permission(Feature.SEARCH, Action = Data.Permission.Action.EDIT)]
        public LuceneService.IndexStat Enable(ApiCall call)
        {
            var site = call.Context.WebSite;
            if (!site.EnableFullTextSearch)
            {
                site.EnableFullTextSearch = true;
                Data.Config.AppHost.SiteRepo.AddOrUpdate(site);
            }
            var rebuild = call.GetValue<bool>("rebuild");

            if (rebuild)
            {
                site.SiteDb().SearchIndex.Rebuild();
            }

            var stat = site.SiteDb().SearchIndex.LuceneService.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat;
        }

        [Permission(Feature.SEARCH, Action = Data.Permission.Action.EDIT)]
        public void Disable(ApiCall call)
        {
            var site = call.Context.WebSite;
            if (site.EnableFullTextSearch)
            {
                site.EnableFullTextSearch = false;
                Data.Config.AppHost.SiteRepo.AddOrUpdate(site);
            }
        }

        [Permission(Feature.SEARCH, Action = Data.Permission.Action.EDIT)]
        public LuceneService.IndexStat Rebuild(ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            siteDb.SearchIndex.Rebuild();
            var stat = siteDb.SearchIndex.LuceneService.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat;
        }

        [Permission(Feature.SEARCH, Action = Data.Permission.Action.DELETE)]
        public LuceneService.IndexStat Clean(ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            siteDb.SearchIndex.DelSelf();
            var stat = siteDb.SearchIndex.LuceneService.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat;
        }

        [Permission(Feature.SEARCH, Action = Data.Permission.Action.VIEW)]
        public LuceneService.IndexStat IndexStat(ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            var stat = siteDb.SearchIndex.LuceneService.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat;
        }

        [Permission(Feature.SEARCH, Action = Data.Permission.Action.VIEW)]
        public List<SearchLog> Lastest(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            return sitedb.SearchIndex.LastestSearch(50);

            //List<SearchLog> fake = new List<SearchLog>();
            //fake.Add(new SearchLog() { Keywords = "long live", IP = "234.234.11.23", DocFound = 123, Time = System.DateTime.Now, ResultCount = 20 });

            //fake.Add(new SearchLog() { Keywords = "animal", IP = "234.234.44.23", DocFound = 14, Time = System.DateTime.Now, ResultCount = 20 });

            //fake.Add(new SearchLog() { Keywords = "kooboo", IP = "234.44.44.23", DocFound = 16, Time = System.DateTime.Now, ResultCount = 20 });

            // return fake;  
        }

        [Permission(Feature.SEARCH, Action = Data.Permission.Action.VIEW)]
        public Dictionary<string, int> SearchStat(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var sitedb = call.Context.WebSite.SiteDb();
            return sitedb.SearchIndex.SearchCount(weekname);
        }

        [Permission(Feature.SEARCH, Action = Data.Permission.Action.VIEW)]
        public List<string> WeekNames(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            return sitedb.SearchIndex.GetWeekNames();
        }
    }
}
