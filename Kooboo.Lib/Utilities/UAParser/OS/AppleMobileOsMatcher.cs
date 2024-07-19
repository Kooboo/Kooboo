using System;
namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public class AppleMobileOsMatcher : IOSMatcher
    {
        public AppleMobileOsMatcher()
        {
        }

        public string OSName => "IOS";

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("like Mac OS", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}