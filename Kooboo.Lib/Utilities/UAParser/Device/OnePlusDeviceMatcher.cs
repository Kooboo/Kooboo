using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class OnePlusDeviceMatcher : BaseDeviceMatcher
    {
        public OnePlusDeviceMatcher()
        {
        }

        public override string DeviceName => "OnePlus";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "One",
            "ONEPLUS"
        };
    }
}

