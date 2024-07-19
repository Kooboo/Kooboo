using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class WebBrowser360SEMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "360 Browser";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "360",
            "360 SE",
            "QIHU",
            "360EE",
        };
    }
}