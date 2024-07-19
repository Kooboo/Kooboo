using System.Collections.Generic;
using System.Net;

namespace Kooboo.Lib.DnsRequest
{
    public class MXRecord
    {
        public int preference { get; set; }

        public string exchange { get; set; }

        public IPAddress IpAddress { get; set; }

        public override string ToString()
        {
            return "Preference : " + preference + " Exchange : " + exchange;
        }
    }

    public class ARecord
    {
        public IPAddress IpAddress { get; set; }

        public string Host { get; set; }

        public List<string> CNames { get; set; }
    }

}
