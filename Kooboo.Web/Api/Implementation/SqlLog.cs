using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class SqlLog : IApi
    {
        public string ModelName
        {
            get { return "SqlLog"; }
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

        private int PageSize = 50;

        [Permission(Feature.DATABASE, Action = "log")]
        public PagedListViewModel<Data.Logging.SqlLog> List(ApiCall apiCall)
        {
            var pageIndex = apiCall.GetIntValue("pageIndex");

            var list = SqlLogService.QueryByWeek(
                apiCall.GetValue("week"),
                apiCall.GetValue("keyword"),
                apiCall.GetValue("type"),
                apiCall.WebSite.Id,
                pageIndex, PageSize, out var total);

            return new PagedListViewModel<Data.Logging.SqlLog>
            {
                List = list,
                PageNr = pageIndex,
                PageSize = PageSize,
                TotalCount = total,
                TotalPages = (total / PageSize) + (total % PageSize > 0 ? 1 : 0)
            };
        }

        [Permission(Feature.DATABASE, Action = "log")]
        public List<KeyValuePair<string, string>> Weeks(ApiCall apiCall)
        {
            var weeks = SqlLogService.GetWeeks(apiCall.WebSite.Id);
            var result = new List<KeyValuePair<string, string>>();
            foreach (var item in weeks)
            {
                var (year, week) = DateTimeHelper.ParseYearWeek(item);
                var (start, end) = DateTimeHelper.GetDateRangeFromWeek(year, week);
                result.Add(new KeyValuePair<string, string>(item, $"{start:yyyy-MM-dd}~{end:yyyy-MM-dd}"));
            }
            return result;
        }
    }
}
