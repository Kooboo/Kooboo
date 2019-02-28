//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System.Collections.Generic;
using Kooboo.Search;

namespace Kooboo.Web.Api.Implementation
{
    public class SearchApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Search";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        [Kooboo.Attributes.RequireParameters("rebuild")]
        public IndexStat Enable(ApiCall call)
        {
            var site = call.Context.WebSite;
            if (!site.EnableFullTextSearch)
            {
                site.EnableFullTextSearch = true;
                Data.GlobalDb.WebSites.AddOrUpdate(site);
            }
            var rebuild = call.GetValue<bool>("rebuild");

            if (rebuild)
            {
                site.SiteDb().SearchIndex.Rebuild();
            }

            var stat = call.WebSite.SiteDb().SearchIndex.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat;

        }

        public void Disable(ApiCall call)
        {
            var site = call.Context.WebSite;
            if (site.EnableFullTextSearch)
            {
                site.EnableFullTextSearch = false;
                Data.GlobalDb.WebSites.AddOrUpdate(site);
            }
        }

        public IndexStat Rebuild(ApiCall call)
        {
            call.Context.WebSite.SiteDb().SearchIndex.Rebuild(); 
            var stat = call.WebSite.SiteDb().SearchIndex.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat; 
        }

        public IndexStat Clean(ApiCall call)
        {
            call.Context.WebSite.SiteDb().SearchIndex.DelSelf();
            var stat = call.WebSite.SiteDb().SearchIndex.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat;
        }

        public IndexStat IndexStat(ApiCall call)
        {
            var stat = call.WebSite.SiteDb().SearchIndex.GetIndexStat();
            stat.EnableFullTextSearch = call.Context.WebSite.EnableFullTextSearch;
            return stat;
        }

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

        public Dictionary<string, int> SearchStat(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var sitedb = call.Context.WebSite.SiteDb();
            return sitedb.SearchIndex.SearchCount(weekname);
        }

        public List<string> WeekNames(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            return sitedb.SearchIndex.GetWeekNames();
        }
    }
}
