using System;
namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public class IPadOsMatcher : IOSMatcher
    {
        public IPadOsMatcher()
        {
        }

        public string OSName => "IPAD";

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("Ipad", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}

