using System;

namespace Kooboo.Lib.Utilities.UAParser.Platform
{
    public class DesktopPlatformMatcher : IPlatformMatcher
    {
        public DesktopPlatformMatcher()
        {
        }

        public string PlatformName => "Desktop";

        public int Preference => 999;

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return false;

            if (userAgentString.Contains("Windows", StringComparison.OrdinalIgnoreCase)
                || userAgentString.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
                return true;
            else if (userAgentString.Contains("X11", StringComparison.OrdinalIgnoreCase)
                || userAgentString.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}

