using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IEnumerable<string> Weeks()
        {
            return SqlLogService.GetWeeks();
        }
    }
}
