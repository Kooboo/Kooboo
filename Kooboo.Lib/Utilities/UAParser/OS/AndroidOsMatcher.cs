namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public class AndroidOsMatcher : IOSMatcher
    {
        public AndroidOsMatcher()
        {
        }

        public string OSName => "Android";

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("Android", System.StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}

