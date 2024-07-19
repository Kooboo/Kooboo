using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Kooboo.Mail.MassMailing
{
    public static class DNSServerProvider
    {
        static DNSServerProvider()
        {
            Servers = GetDnsServers();
        }

        private static List<IPAddress> _dns;

        public static List<IPAddress> Servers
        {
            get
            {
                if (_dns == null)
                {
                    var list = GetDnsServers();
                    _dns = list;
                }
                return _dns;
            }

            set
            {
                _dns = value;
            }

        }

        public static List<IPAddress> GetDnsServers()
        {
            List<IPAddress> DnsServer = new List<IPAddress>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var type = networkInterface.NetworkInterfaceType;

                    if (type == NetworkInterfaceType.Ethernet || type == NetworkInterfaceType.Ethernet3Megabit || type == NetworkInterfaceType.FastEthernetFx || type == NetworkInterfaceType.FastEthernetT || type == NetworkInterfaceType.GigabitEthernet || type == NetworkInterfaceType.Wireless80211)
                    {

                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                        if (ipProperties != null)
                        {
                            IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                            foreach (IPAddress dnsAddress in dnsAddresses)
                            {
                                if (dnsAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || dnsAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                {
                                    DnsServer.Add(dnsAddress);
                                }
                            }
                        }
                    }

                }
            }

            DnsServer.Add(System.Net.IPAddress.Parse("1.1.1.1"));

            DnsServer.Add(System.Net.IPAddress.Parse("8.8.8.8"));

            return DnsServer;
        }
    }
}
