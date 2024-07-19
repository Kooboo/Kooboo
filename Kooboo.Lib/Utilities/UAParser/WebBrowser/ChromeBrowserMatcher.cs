using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class ChromeBrowserMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Chrome";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Chrome"
        };
    }
}