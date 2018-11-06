//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class VisitorLogApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "VisitorLog";
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

        public virtual PagedListViewModel<VisitorLog> List(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var log = call.WebSite.SiteDb().VisitorLog;
            if (!string.IsNullOrEmpty(weekname))
            {
                log = call.WebSite.SiteDb().LogByWeek<VisitorLog>(weekname);
            }

            var pager = ApiHelper.GetPager(call, 50);   

            PagedListViewModel<VisitorLog> result = new PagedListViewModel<VisitorLog>();

            var alllog = log.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead);  

            var total = alllog.Count();
            result.TotalCount = total;
            result.TotalPages = ApiHelper.GetPageCount(total, pager.PageSize);
            result.PageNr = pager.PageNr;

            List<VisitorLog> logs = alllog.Skip(pager.SkipCount).Take(pager.PageSize).ToList();
            result.List = logs;
            return result;   
        }

        public List<ResourceCount> TopPages(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            return Kooboo.Sites.Service.VisitorLogService.TopPages(call.WebSite.SiteDb(), weekname).Take(100).ToList();
        }

        public List<string> WeekNames(ApiCall call)
        {
            return call.WebSite.SiteDb().LogWeekNames();
        }

        public List<ResourceCount> TopReferer(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var result = VisitorLogService.TopReferers(call.WebSite.SiteDb(), weekname).Take(100).ToList();
            return result;
        }

        public List<ResourceCount> TopReferUrl(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var result = VisitorLogService.TopReferUrl(call.WebSite.SiteDb(), weekname).Take(100).ToList();
            return result;
        }

        public List<ImageLogItemViewModel> TopImages(ApiCall call)
        {
            List<ImageLogItemViewModel> result = new List<ImageLogItemViewModel>();
            string weekname = call.GetValue("weekname");
            var imagelogs = VisitorLogService.GetImageLogs(call.WebSite.SiteDb(), weekname);

            string baseurl = call.WebSite.BaseUrl();

            foreach (var item in imagelogs.GroupBy(o => o.ImageId))
            {
                string url = item.First().Url;
                ImageLogItemViewModel model = new ImageLogItemViewModel();
                model.Name = url;
                model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Name);
                model.Size = item.First().Size;
                model.Count = item.Count();
                model.ThumbNail = Sites.Service.ThumbnailService.GenerateThumbnailUrl(item.Key, 50, 50, call.WebSite.Id);
                result.Add(model);
            }
            return result.OrderByDescending(o => o.Count).Take(100).ToList();
        }

        public List<ErrorSummaryViewModel> ErrorList(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var sitedb = call.WebSite.SiteDb();

            List<ErrorSummaryViewModel> errors = new List<ErrorSummaryViewModel>();
            Sequence<SiteErrorLog> repo = string.IsNullOrEmpty(weekname) ? sitedb.ErrorLog : sitedb.LogByWeek<SiteErrorLog>(weekname);
            string baseurl = call.WebSite.BaseUrl();
            var allitems = repo.AllItemList();

            foreach (var item in allitems.GroupBy(o => o.Id))
            {
                ErrorSummaryViewModel model = new ErrorSummaryViewModel();
                model.Id = item.Key;
                model.Count = item.Count();
                model.Url = item.First().Url;
                try
                {
                    model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Url);
                }
                catch (Exception)
                {
                    model.PreviewUrl = baseurl;
                }

                errors.Add(model);
            }
            return errors.OrderByDescending(o => o.Count).ToList();
        }

        [Kooboo.Attributes.RequireParameters("id")]
        public List<SiteErrorLog> ErrorDetail(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var sitedb = call.WebSite.SiteDb();

            List<ErrorSummaryViewModel> errors = new List<ErrorSummaryViewModel>();

            Sequence<SiteErrorLog> repo = string.IsNullOrEmpty(weekname) ? sitedb.ErrorLog : sitedb.LogByWeek<SiteErrorLog>(weekname);

            string baseurl = call.WebSite.BaseUrl();
            return repo.QueryDescending(o => o.Id == call.ObjectId).Take(99999).ToList();

        }

        public List<ResourceCount> Monthly(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            return VisitorLogService.MonthlyVisitors(sitedb);
        }
    }
}
