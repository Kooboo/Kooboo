using System.Collections.Generic;

namespace Kooboo.Mail.Models
{
    public class SendSetting
    {
        public bool OkToSend { get; set; }

        public string ErrorMessage { get; set; }

        public string HostName { get; set; }

        public System.Net.IPAddress LocalIp { get; set; }

        public bool UseKooboo { get; set; }

        public int Port { get; set; } = 25;

        public string KoobooServerIp { get; set; }

        public List<string> Mxs { get; set; }
    }
}