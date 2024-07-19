using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class MicrosoftEdgeMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Microsoft Edge";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Edge",
            "Edg"
        };
    }
}