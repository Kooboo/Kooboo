using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class AppleDeviceMatcher : BaseDeviceMatcher
    {
        public AppleDeviceMatcher()
        {
        }

        public override string DeviceName => "Apple";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "iPhone",
            "iPad",
            "Apple",
            "Macintosh"
        };
    }
}

