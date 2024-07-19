using System.Collections.Generic;
namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class MiDeviceMatcher : BaseDeviceMatcher
    {
        public MiDeviceMatcher()
        {
        }

        public override string DeviceName => "XiaoMi";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Mi",
            "HM",
            "MI-ONE",
            "Redmi",
            "MIX",
            "M2101K6G",
            "M2102J20SG",
            "2201116SG",
        };
    }
}

