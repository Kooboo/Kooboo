using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class SamsungDeviceMatcher : BaseDeviceMatcher
    {
        public SamsungDeviceMatcher()
        {
        }

        public override string DeviceName => "Samsung";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "samsung",
            "SCH",
            "SGH",
            "SHV",
            "SHW",
            "SPH",
            "SC",
            "SM",
            "EK-GC100",
            "SCL21",
            "I9300",
        };
    }
}

