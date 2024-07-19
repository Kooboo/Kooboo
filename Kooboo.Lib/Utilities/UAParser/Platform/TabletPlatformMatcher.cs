using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Platform
{
    public class TabletPlatformMatcher : IPlatformMatcher
    {
        public TabletPlatformMatcher()
        {
        }

        public string PlatformName => "Tablet";

        public int Preference => 1;

        public List<string> SpecialTabletIdentification = new List<string>()
        {
            "SM-X906C",
            "Lenovo YT-J706X",
            "Pixel C",
            "SGP771",
            "SM-T827R4",
            "SAMSUNG SM-T550",
            "KFTHWI",
            "LG-V410"
        };

        public bool Match(string userAgentString)
        {
            if (userAgentString.Contains("Tablet", System.StringComparison.OrdinalIgnoreCase))
                return true;

            if (userAgentString.Contains("Ipad", System.StringComparison.OrdinalIgnoreCase))
                return true;

            if (userAgentString.Contains("Pad", System.StringComparison.OrdinalIgnoreCase))
                return true;

            var find = SpecialTabletIdentification
                .Find(o => userAgentString.Contains(o, System.StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(find))
                return true;


            return false;
        }
    }
}

