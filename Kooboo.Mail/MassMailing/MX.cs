using System.Net;

namespace Kooboo.Mail.MassMailing
{
    public class MX
    {
        public int Preference { get; set; }

        public string Domain { get; set; }

        public IPAddress IP { get; set; }
    }
}
