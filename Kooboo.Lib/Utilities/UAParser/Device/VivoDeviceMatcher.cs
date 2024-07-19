using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class VivoDeviceMatcher : BaseDeviceMatcher
    {
        public override string DeviceName => "Vivo";

        public override List<string> SpecialIdentification => new List<string>()
        {
            "vivo"
        };
    }

}