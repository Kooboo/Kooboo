using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Whois
{
    public class WhoisRecord
    {

        public DateTime Expiration { get; set; }

        public List<string> NameServers { get; set; }
    }
}
