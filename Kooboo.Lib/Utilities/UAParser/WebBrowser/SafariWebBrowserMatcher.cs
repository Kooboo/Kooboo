using System.Collections.Generic;
namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public class SafariWebBrowserMatcher : BaseWebBrowserMatcher
    {
        public override string BrowserName => "Safari";

        public override List<string> SpecialIdentification => new List<string>()
        {
        };

        public override bool TryMatch(string userAgentString, out ApplicationInfo application)
        {
            if (userAgentString.Contains("Macintosh")
            && userAgentString.Contains("AppleWebKit")
            && userAgentString.Contains("Safari"))
            {
                var version = GetVersion("Safari", userAgentString);
                application = new ApplicationInfo(BrowserName, version);
                return true;
            }

            application = null;
            return false;
        }
    }
}