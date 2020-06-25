//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Service
{
    public static class VisitorLogService
    {      

        public static SiteVisitorOverview GetOverView(SiteDb sitedb, string weekname = null)
        {
            var logs = GetLogs(sitedb, weekname);
            SiteVisitorOverview model = new SiteVisitorOverview();
            model.Total = logs.Count();
            model.Ips = logs.GroupBy(o => o.ClientIP).Count();
            model.Pages = logs.GroupBy(o => o.ObjectId).Count();
            model.AvgSize = (int)logs.Average(o => o.Size);
            return model;
        }

        public static List<ResourceCount> TopReferers(SiteDb sitedb, string weekname = null)
        {
            var sitebindings = Kooboo.Data.GlobalDb.Bindings.GetByWebSite(sitedb.WebSite.Id);
            List<string> ownnames = new List<string>();
            foreach (var item in sitebindings)
            {
                ownnames.Add(item.FullName);
            }

            List<string> domainlist = new List<string>();

            var logs = GetLogs(sitedb, weekname);

            foreach (var item in logs)
            {
                if (!string.IsNullOrEmpty(item.Referer))
                {
                    string host = Kooboo.Lib.Helper.UrlHelper.UriHost(item.Referer, true);
                    if (!string.IsNullOrEmpty(host))
                    {
                        if (!ownnames.Contains(host, StringComparer.OrdinalIgnoreCase))
                        {
                            domainlist.Add(item.Referer);
                        }
                    }
                }
            }

            List<ResourceCount> referercount = new List<ResourceCount>();

            foreach (var item in domainlist.GroupBy(o => o))
            {
                ResourceCount count = new ResourceCount();
                count.Name = item.Key;
                count.Count = item.Count();
                referercount.Add(count);
            }
            return referercount.OrderByDescending(o => o.Count).ToList();
        }

        public static List<ResourceCount> TopReferUrl(SiteDb sitedb, string weekname = null)
        {
            var sitebindings = Kooboo.Data.GlobalDb.Bindings.GetByWebSite(sitedb.WebSite.Id);
            List<string> ownnames = new List<string>();
            foreach (var item in sitebindings)
            {
                ownnames.Add(item.FullName);
            }

            List<string> domainlist = new List<string>();

            var logs = GetLogs(sitedb, weekname);

            foreach (var item in logs)
            {
                if (!string.IsNullOrEmpty(item.Referer))
                {
                    if (!ownnames.Contains(item.Referer, StringComparer.OrdinalIgnoreCase))
                    {
                        domainlist.Add(item.Referer);
                    }
                }
            }

            List<ResourceCount> referercount = new List<ResourceCount>();

            foreach (var item in domainlist.GroupBy(o => o))
            {
                ResourceCount count = new ResourceCount();
                count.Name = item.Key;
                count.Count = item.Count();
                referercount.Add(count);
            }
            return referercount.OrderByDescending(o => o.Count).ToList();
        }
        
        public static List<ResourceCount> TopPages(SiteDb sitedb, string WeekName = null)
        {
            List<ResourceCount> pagecountes = new List<ResourceCount>();
            var logs = GetLogs(sitedb, WeekName);
            foreach (var item in logs.GroupBy(o => o.ObjectId))
            {
                var objectid = item.Key;
                var page = sitedb.Pages.Get(objectid, true);
                if (page != null)
                {
                    ResourceCount count = new ResourceCount();
                    var pageurl = ObjectService.GetObjectRelativeUrl(sitedb, objectid, ConstObjectType.Page);
                    count.Name = pageurl;
                    count.Count = item.Count();
                    count.Size = item.First().Size;
                    pagecountes.Add(count);
                }
            }

            return pagecountes.OrderByDescending(o => o.Count).ToList();
        }

        public static List<VisitorLog> GetLogs(SiteDb sitedb, string WeekName = null)
        {
            if (string.IsNullOrEmpty(WeekName))
            {
                return sitedb.VisitorLog.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead);  
            }
            else
            {
                var repo = sitedb.LogByWeek<VisitorLog>(WeekName);
                var list = repo.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead); 
                repo.Close();
                return list;
            }
        }

        public static List<ImageLog> GetImageLogs(SiteDb sitedb, string WeekName = null)
        {
            if (string.IsNullOrEmpty(WeekName))
            {
                return sitedb.ImageLog.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead); 
               // return sitedb.ImageLog.AllItemList();
            }
            else
            {
                var store = sitedb.LogByWeek<ImageLog>(WeekName);
                var list = store.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead);  
                //var list = store.AllItemList();
                store.Close();
                return list;
            }
        }

        public static List<SiteErrorLog> GetErrorLogs(SiteDb sitedb, string WeekName = null)
        {
            if (string.IsNullOrEmpty(WeekName))
            {
                return sitedb.ErrorLog.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead); 
                //return sitedb.ErrorLog.AllItemList();
            }
            else
            {
                var store = sitedb.LogByWeek<SiteErrorLog>(WeekName);
                var list = store.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead); 
                //var list = store.AllItemList();
                store.Close();
                return list;
            }
        }

        public static List<ResourceCount> TopImages(SiteDb sitedb, string WeekName = null)
        {
            List<ResourceCount> imagecounts = new List<ResourceCount>();
            var logs = GetImageLogs(sitedb, WeekName);
            foreach (var item in logs.GroupBy(o => o.Url, StringComparer.OrdinalIgnoreCase))
            {
                string url = item.Key;
                if (!string.IsNullOrEmpty(url))
                {
                    ResourceCount count = new ResourceCount();
                    count.Name = url;
                    count.Count = item.Count();
                    count.Size = item.First().Size;
                    imagecounts.Add(count);
                }
            }
            return imagecounts.OrderByDescending(o => o.Count).ToList();
        } 

        public static List<ResourceCount> MonthlyVisitors(SiteDb sitedb)
        {
            var allweeks = sitedb.LogWeekNames();

            var lastfour = allweeks.OrderByDescending(o => o).Take(4);

            List<ResourceCount> monthly = new List<ResourceCount>();

            List<VisitorLog> logs = new List<VisitorLog>();

            foreach (var item in lastfour)
            {
                logs.AddRange(GetLogs(sitedb, item));
            }

            var groupby = logs.GroupBy(o => GetDayString(o.Begin));

            foreach (var item in groupby)
            {
                ResourceCount resouce = new ResourceCount();
                resouce.Name = item.Key;
                resouce.Count = item.Count();
                monthly.Add(resouce);
            }

            return monthly.OrderBy(o => o.Name).ToList();
        }

        private static string GetDayString(DateTime datetime)
        {
            int year = datetime.Year;
            int month = datetime.Month;
            int day = datetime.Day;

            string result = year.ToString();
            if (month < 10)
            { result += "-0" + month.ToString(); }
            else
            { result += "-" + month.ToString(); }


            if (day < 10)
            {
                result += "-0" + day.ToString();
            }
            else
            {
                result += "-" + day.ToString();
            }

            return result;
        }

    }

    public class SiteVisitorOverview
    {
        public int Total { get; set; }
        public int Ips { get; set; }

        public int Pages { get; set; }

        public int AvgSize { get; set; }
    }

    public class ResourceCount
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public long Size { get; set; } 
    }

}
