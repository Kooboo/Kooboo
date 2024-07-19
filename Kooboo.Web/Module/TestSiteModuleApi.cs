//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Module;

namespace Kooboo.Web.Modules
{
    public class TestSiteModuleApi : ISiteModuleApi
    {
        public string ModelName => "WebTestSample";

        public Dictionary<string, string> methodone(ApiCall call)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            response.Add("callfrom", call.Context.Request.IP);
            if (call.WebSite != null)
            {
                response.Add("sitename", call.WebSite.Name);
            }
            return response;
        }
    }
}
