using System;
namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public class MacOsMatcher : IOSMatcher
    {
        public MacOsMatcher()
        {
        }

        public string OSName => "Mac";

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}

