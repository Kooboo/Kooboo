using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class HonorDeviceMatcher : BaseDeviceMatcher
    {
        public HonorDeviceMatcher()
        {
        }

        public override string DeviceName => "Honor";


        public override List<string> SpecialIdentification => new List<string>()
        {
            "honor"
        };
    }
}

