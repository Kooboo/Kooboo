using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.WebBrowser
{
    public abstract class BaseWebBrowserMatcher : IWebBrowserMatcher
    {
        public abstract string BrowserName { get; }

        public abstract List<string> SpecialIdentification { get; }

        public virtual bool TryMatch(string userAgentString, out ApplicationInfo application)
        {
            var find = SpecialIdentification
                        .Find(o => userAgentString.Contains(o, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(find))
            {
                // get version
                var version = GetVersion(find, userAgentString);
                if (string.IsNullOrEmpty(version))
                    version = "0";

                application = new ApplicationInfo(BrowserName, version);
                return true;
            }

            application = null;
            return false;
        }

        public virtual string GetVersion(string identifier, string userAgentString)
        {
            var startIndex = userAgentString.IndexOf(identifier, StringComparison.OrdinalIgnoreCase);
            if (startIndex > 0)
            {
                var subString = userAgentString.Substring(startIndex);
                var endIndex = subString.IndexOf(' ');
                string webBrowserDescription = string.Empty;
                if (endIndex > 0)
                    webBrowserDescription = userAgentString.Substring(startIndex, endIndex);
                else
                    webBrowserDescription = userAgentString.Substring(startIndex);
                var infoSpans = webBrowserDescription.Split('/');
                if (infoSpans.Length > 1)
                {
                    if (!string.IsNullOrEmpty(infoSpans[1]))
                        return infoSpans[1];
                }
            }
            return "0";
        }
    }
}