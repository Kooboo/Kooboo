using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class OppoDeviceMatcher : BaseDeviceMatcher
    {
        public OppoDeviceMatcher()
        {
        }

        public override string DeviceName => "Oppo";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "Oppo",
            "CPH",
            "A1601",
            "RMX",
            "R800",
            "R811",
            "R8123",
            "R820",
            "R829",
            "T703",
            "U70",
            "U71",
            "U72",
            "X900",
            "X901",
            "X9023",
            "X910",
            "X919",
            "AFR",
        };
    }
}

