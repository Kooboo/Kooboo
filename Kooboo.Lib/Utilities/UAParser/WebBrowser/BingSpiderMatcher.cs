using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class BingSpiderMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Bing bot";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "msnbot",
            "bingbot",
            "bing.com",
            "bing",
            "BingPreview",
        };
    }
}