using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public abstract class BaseDeviceMatcher : IDeviceMatcher
    {
        public BaseDeviceMatcher()
        {
        }

        public abstract string DeviceName { get; }

        public abstract List<string> SpecialIdentification { get; }

        public bool Match(string userAgentString)
        {
            var find = SpecialIdentification
               .Find(o => userAgentString.Contains(o, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(find))
                return true;

            return false;
        }
    }
}

