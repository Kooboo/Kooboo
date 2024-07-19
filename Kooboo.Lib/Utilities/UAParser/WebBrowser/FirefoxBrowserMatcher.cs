using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class FirefoxBrowserMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Firefox";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Firefox",
        };
    }
}