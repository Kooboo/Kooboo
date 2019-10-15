//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Data.Models
{
    public class ServerSetting
    {
        public string HostDomain { get; set; }

        public int SmtpPort { get; set; } = 587;

        public int OnlineDataCenterId { get; set; }

        public int ServerId { get; set; }

        // in case that this is a an Dns server.
        public string NameServer { get; set; }

        public string ServerName { get; set; }

        public string SmtpServerIP { get; set; }

        public string ThemeDomain { get; set; }

        private List<ReverseDns> _reversedns;

        public List<ReverseDns> ReverseDns
        {
            get => _reversedns ?? (_reversedns = new List<Models.ReverseDns>());
            set => _reversedns = value;
        }

        private HashSet<string> _ips;

        public HashSet<string> Ips
        {
            get => _ips ?? (_ips = new HashSet<string>());
            set => _ips = value;
        }

        // Dns server used for people to set their own domain.
        public string Ns1 { get; set; }

        public string Ns2 { get; set; }

        public string MyIP { get; set; }

        public ServerType ServerType { get; set; }
    }

    public class ReverseDns
    {
        public string IP { get; set; }

        public string HostName { get; set; }
    }
}