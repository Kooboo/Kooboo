using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class GoogleBotMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Google Bot";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Googlebot",
            "google.com/bot",
            "Google-AMPHTML"
        };
    }
}