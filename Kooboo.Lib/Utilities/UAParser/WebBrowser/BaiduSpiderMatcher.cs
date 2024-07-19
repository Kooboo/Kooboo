using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class BaiduSpiderMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Baidu Spider";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "baidu",
            "Baiduspider",
            "M2011K2C",
        };
    }
}