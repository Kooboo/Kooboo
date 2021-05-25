using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Web.Api.Implementation
{
    public class CodeSearch : IApi
    {
        public string ModelName => "CodeSearch";

        public bool RequireSite => true;

        public bool RequireUser => false;

        public object List(ApiCall apiCall)
        {
            var keyword = apiCall.GetValue("keyword");

            // TODO use keyword to search

            var list = new[] {
                new {Name="siteNavView", Type="View",Url="/view?id=xxx",Line=23},
                new {Name="login", Type="Code",Url="/code?id=xxx",Line=12},
                new {Name="logout", Type="View",Url="/view?id=xxx",Line=23},
                new {Name="footer", Type="View",Url="/view?id=xxx",Line=23},
                new {Name="topmenu", Type="View",Url="/view?id=xxx",Line=23},
                new {Name="siteNavView", Type="View",Url="/view?id=xxx",Line=23},
                new {Name="siteNavView", Type="View",Url="/view?id=xxx",Line=23},
            };

            if (!string.IsNullOrWhiteSpace(keyword)) return list.Where(w => w.Name.Contains(keyword)).ToArray();
            return list;
        }
    }
}
