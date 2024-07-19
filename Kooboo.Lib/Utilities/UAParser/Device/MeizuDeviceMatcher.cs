using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class MeizuDeviceMatcher : BaseDeviceMatcher
    {
        public override string DeviceName => "Meizu";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "meizu",
            "M030",
            "M031",
            "M035",
            "M040",
            "M065",
            "m9"
        };
    }

}