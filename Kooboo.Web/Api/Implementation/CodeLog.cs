using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Scripting.Global.Logging;

namespace Kooboo.Web.Api.Implementation
{
    public class CodeLogApi : IApi
    {
        public string ModelName => "CodeLog";

        public bool RequireSite => true;

        public bool RequireUser => true;

        [Permission(Feature.CODE, Action = "log")]
        public PagedList<Data.Logging.CodeLog> Query(ApiCall call, CodeLogQuery query)
        {
            string weekname = call.GetValue("weekname");
            return LogService.QueryLog(call.WebSite, query,weekname);
        }

        [Permission(Feature.CODE)]
        public List<KeyValuePair<string, string>> WeekNames(ApiCall call)
        {
            var weeks= LogService.LogWeekNames(call.WebSite.SiteDb());
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
