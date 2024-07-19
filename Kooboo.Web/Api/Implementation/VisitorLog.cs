//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Data.Permission;
using Kooboo.Data.Storage;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

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

        [Permission(Feature.VISITOR_LOG)]
        public virtual PagedListViewModel<VisitorLog> List(ApiCall call)
        {
            string weekname = call.GetValue("weekname");

            var siteDb = call.Context.WebSite.SiteDb();

            var logStore = siteDb.VisitorLogByWeek(weekname);

            var pager = ApiHelper.GetPager(call, 50);

            PagedListViewModel<VisitorLog> result = new PagedListViewModel<VisitorLog>();

            var logResult = logStore.List(pager.PageNr, pager.PageSize);

            result.TotalCount = logResult.TotalCount;
            result.TotalPages = logResult.TotalPages;
            result.PageNr = pager.PageNr;
            result.List = logResult.DataList.ToList();
            return result;
        }

        [Permission(Feature.VISITOR_LOG)]
        public virtual PagedListViewModel<VisitorLog> BotList(ApiCall call)
        {
            string weekname = call.GetValue("weekname");

            var siteDb = call.Context.WebSite.SiteDb();

            var logStore = siteDb.BotLogByWeek(weekname);

            var pager = ApiHelper.GetPager(call, 50);

            PagedListViewModel<VisitorLog> result = new PagedListViewModel<VisitorLog>();

            var logResult = logStore.List(pager.PageNr, pager.PageSize);

            result.TotalCount = logResult.TotalCount;
            result.TotalPages = logResult.TotalPages;
            result.PageNr = pager.PageNr;
            result.List = logResult.DataList?.ToList() ?? [];
            return result;
        }

        [Permission(Feature.VISITOR_LOG)]
        public List<ResourceCount> TopBots(ApiCall call)
        {
            return Kooboo.Sites.Service.VisitorLogService.TopBots(call.WebSite.SiteDb(), 100, call.GetValue("weekname"));
        }


        [Permission(Feature.VISITOR_LOG)]
        public List<ResourceCount> TopPages(ApiCall call)
        {
            return Kooboo.Sites.Service.VisitorLogService.TopPages(call.WebSite.SiteDb(), 100, call.GetValue("weekname"));
        }

        [Permission(Feature.VISITOR_LOG)]
        public IEnumerable<string> WeekNames(ApiCall call)
        {
            return call.WebSite.SiteDb().VisitorLogWeekNames();
        }

        [Permission(Feature.VISITOR_LOG)]
        public List<ResourceCount> TopReferer(ApiCall call)
        {

            string weekname = call.GetValue("weekname");
            var result = VisitorLogService.TopReferrers(call.WebSite.SiteDb(), weekname).Take(100).ToList();
            return result;
        }

        [Permission(Feature.VISITOR_LOG)]
        public List<ResourceCount> TopReferUrl(ApiCall call)
        {
            string weekname = call.GetValue("weekname");
            var result = VisitorLogService.TopReferUrl(call.WebSite.SiteDb(), weekname).Take(100).ToList();
            return result;
        }

        [Permission(Feature.VISITOR_LOG)]
        public List<ImageLogItemViewModel> TopImages(ApiCall call)
        {
            List<ImageLogItemViewModel> result = new List<ImageLogItemViewModel>();
            string weekName = call.GetValue("weekname");

            var siteDb = call.Context.WebSite.SiteDb();

            var logStore = siteDb.ImageLogByWeek(weekName);

            string baseUrl = call.WebSite.BaseUrl();

            int counter = 0;

            foreach (var item in logStore.TopUrl)
            {
                string url = item.Key;

                ImageLogItemViewModel model = new ImageLogItemViewModel();
                model.Name = url;
                model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseUrl, model.Name);

                var image = siteDb.Images.GetByUrl(url);
                if (image != null)
                {
                    model.Size = image.Size;
                }

                model.Count = item.Value;
                model.ThumbNail = Sites.Service.ThumbnailService.GenerateThumbnailUrl(item.Key, 50, 50, call.WebSite.Id);
                result.Add(model);

                counter += 1;
                if (counter > 100)
                {
                    break;
                }
            }
            return result.ToList();
        }

        [Permission(Feature.VISITOR_LOG)]
        public List<ErrorSummaryViewModel> ErrorList(ApiCall call)
        {
            string weekName = call.GetValue("weekname");

            var sitedb = call.WebSite.SiteDb();

            List<ErrorSummaryViewModel> errors = new List<ErrorSummaryViewModel>();

            var logStore = sitedb.ErrorLogByWeek(weekName);

            string baseUrl = call.WebSite.BaseUrl();

            foreach (var item in logStore.TopErrorUrl)
            {
                ErrorSummaryViewModel model = new ErrorSummaryViewModel();
                model.Id = Lib.Security.Hash.ComputeGuidIgnoreCase(item.Key);
                model.Count = item.Value;

                model.Url = item.Key;
                try
                {
                    model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseUrl, model.Url);
                }
                catch (Exception)
                {
                    model.PreviewUrl = baseUrl;
                }

                errors.Add(model);
            }
            return errors.OrderByDescending(o => o.Count).ToList();
        }

        [Permission(Feature.VISITOR_LOG)]
        public PagedList<SiteErrorLogViewModel> Errors(ApiCall call)
        {
            string weekName = call.GetValue("weekname");

            var sitedb = call.WebSite.SiteDb();

            var logStore = sitedb.ErrorLogByWeek(weekName);

            var result = new PagedList<SiteErrorLogViewModel>();

            var pager = ApiHelper.GetPager(call, 50);

            var logResult = logStore.List(pager.PageNr, pager.PageSize);

            string baseUrl = call.WebSite.BaseUrl();

            List<SiteErrorLogViewModel> DataItems = new List<SiteErrorLogViewModel>();

            if (logResult.DataList != null)
            {
                foreach (var item in logResult.DataList)
                {
                    SiteErrorLogViewModel model = new SiteErrorLogViewModel(item, baseUrl);

                    DataItems.Add(model);
                }
            }

            result.PageNr = pager.PageNr;
            result.PageSize = pager.PageSize;
            result.TotalCount = logResult.TotalCount;
            result.DataList = DataItems.ToArray();

            return result;
        }



        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.VISITOR_LOG)]
        public List<SiteErrorLog> ErrorDetail(ApiCall call)
        {
            string weekName = call.GetValue("weekname");
            var sitedb = call.WebSite.SiteDb();

            List<ErrorSummaryViewModel> errors = new List<ErrorSummaryViewModel>();

            var logStore = sitedb.ErrorLogByWeek(weekName);
            return logStore.ByObjId(call.ObjectId, 499);
        }

        [Permission(Feature.VISITOR_LOG)]
        public List<ResourceCount> Monthly(ApiCall call)
        {

            var sitedb = call.WebSite.SiteDb();
            return VisitorLogService.MonthlyVisitors(sitedb);
        }
    }
}
