using System;
namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public class WindowsOsMatcher : IOSMatcher
    {
        public WindowsOsMatcher()
        {
        }

        public string OSName => "Windows";

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}

