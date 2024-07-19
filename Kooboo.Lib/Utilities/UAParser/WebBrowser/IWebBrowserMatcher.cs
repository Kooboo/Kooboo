using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public interface IWebBrowserMatcher
    {
        string BrowserName { get; }
        List<string> SpecialIdentification { get; }

        public bool TryMatch(string userAgentString, out ApplicationInfo application);
    }
}