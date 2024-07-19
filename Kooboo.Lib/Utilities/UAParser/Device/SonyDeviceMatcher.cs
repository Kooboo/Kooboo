using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class SonyDeviceMatcher : BaseDeviceMatcher
    {
        public SonyDeviceMatcher()
        {
        }

        public override string DeviceName => "Sony";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "sony",
            "Xperia",
            "SGP",
            "SGPT",
            "NW-Z1000Series"
        };
    }
}

