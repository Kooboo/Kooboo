using System;
namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public class LinuxOsMatcher : IOSMatcher
    {
        public LinuxOsMatcher()
        {
        }

        public string OSName => "Linux";

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("Linux", StringComparison.OrdinalIgnoreCase)
                || userAgentString.Contains("X11", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}

