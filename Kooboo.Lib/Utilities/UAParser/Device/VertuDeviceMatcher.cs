using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class VertuDeviceMatcher : BaseDeviceMatcher
    {
        public override string DeviceName => "Vertu";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "VERTU"
        };
    }
}