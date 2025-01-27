using Kooboo.Api;
using Kooboo.Data.Permission;
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
        public IEnumerable<string> WeekNames(ApiCall call)
        {
            return LogService.LogWeekNames(call.WebSite.SiteDb()); ;
        }
    }
}
