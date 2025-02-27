﻿using System.Collections.Generic;

namespace Kooboo.Mail.MassMailing.Model
{

    public class ISP
    {
        public string Name { get; set; }

        public bool StartTls { get; set; }

        public bool PipeLining { get; set; }

        public HashSet<string> SendingCountry { get; set; }   // This ISP can only be sent from certain country...Received Mails will forward to this country. 

        public HashSet<string> RootDomains { get; set; }

        public HashSet<string> MxDomains { get; set; } = new HashSet<string>();

        public List<IPISPQuota> Quota { get; set; } = new List<IPISPQuota>();

        public IPISPQuota GetQuota(string IP)
        {
            var result = this.Quota.Find(o => o.LocalIP == IP);

            return result == null ? new IPISPQuota() { MailsPerConnection = 10, MaxConnections = 10, DailyQuota = 50000 } : result;
        }

    }
}
