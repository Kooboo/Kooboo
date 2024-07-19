//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Context;

namespace Kooboo.Web.JsTest
{
    public class JsTestOption
    {

        public string GetDiskRoot(RenderContext request)
        {
            if (request != null && request.WebSite != null && !string.IsNullOrEmpty(request.WebSite.LocalRootPath))
            {
                return request.WebSite.LocalRootPath;
            }
            return AppSettings.RootPath;
        }


        public bool ShouldTryHandle(Kooboo.Data.Context.RenderContext context, JsTestOption Options)
        {
            if (string.IsNullOrEmpty(Options.RequestPrefix))
            {
                return true;
            }
            string RelativeUrl = context.Request.RawRelativeUrl;

            if (RelativeUrl.ToLower().StartsWith(Options.RequestPrefix.ToLower()))
            {
                return true;
            }
            return false;
        }

        public string FolderPath(RenderContext context)
        {
            if (context.WebSite != null && !string.IsNullOrEmpty(context.WebSite.LocalRootPath))
            {
                return null;
            }
            else
            {
                return "/_admin";
            }
        }

        // this is the prefix url to request for this test function. 
        public string RequestPrefix { get; set; }

        public string TestFolder { get; set; } = "kbtest";

        public string FunctionBlockStart { get; set; } = "/*teststart*/";

        public string FunctionBlockEnd { get; set; } = "/*testend*/";

        public string JsReferenceFileName { get; set; } = "ref.txt";

        public HashSet<string> AssertJs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    }
}
