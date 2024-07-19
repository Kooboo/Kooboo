namespace Kooboo.Lib.Utilities.UAParser.Platform
{
    public class MobilePlatformMatcher : IPlatformMatcher
    {
        public string PlatformName => "Mobile";

        public int Preference => 2;

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("Mobile", System.StringComparison.OrdinalIgnoreCase))
                return true;
            if (userAgentString.Contains("Android", System.StringComparison.OrdinalIgnoreCase))
                return true;
            if (userAgentString.Contains("Ipad", System.StringComparison.OrdinalIgnoreCase))
                return true;
            if (userAgentString.Contains("like Mac OS", System.StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}

