using System.Collections.Generic;
namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public interface IDeviceMatcher
    {
        string DeviceName { get; }
        List<string> SpecialIdentification { get; }

        public bool Match(string userAgentString);
    }
}

