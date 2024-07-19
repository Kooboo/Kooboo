using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class HuaweiDeviceMatcher : BaseDeviceMatcher
    {
        public HuaweiDeviceMatcher()
        {
        }

        public override string DeviceName => "Huawei";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "huawei",
            "HarmonyOS",
            "HMSCore",
            "LYA-AL00P",
            "OCE-AN50",
            "GOT-AL09",
            "ANG-AN00",
            "BRQ-AL00",
            "AQM-AL00",
            "OXF-AN00",
            "LIO-AN00m",
            "ABR-AL60",
            "OXP-AN00",
            "NOH-AN00",
            "PAL-AL00",
            "GOT-W09",
            "ideos",
            "Orange Daytona",
            "Pulse",
            "Vodafone 858",
            "C8500",
            "C8600",
            "C8650",
            "C8660",
            "Nexus 6P",
            "ATH-",
            "M860",
            "VOG-L29",
            "MAR-LX1A"
        };
    }
}

