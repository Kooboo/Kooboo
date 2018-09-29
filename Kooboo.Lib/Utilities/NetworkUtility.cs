//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;
using Microsoft.Win32;

namespace Kooboo.Lib.Utilities
{
    public static class NetworkUtility
    {

        /// <summary>
        /// Set the two dns server, OK agreed that I do not allow null for anyone. 
        /// </summary>
        /// <param name="macAddress"></param>
        /// <param name="primaryDns"></param>
        /// <param name="secondaryDns"></param>
        public static void SetDnsServer(string macAddress, string primaryDns, string secondaryDns)
        {
            if (string.IsNullOrEmpty(primaryDns))
            {
                return;
            }

            var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration");

            var configs = networkConfigMng.GetInstances();

            foreach (var item in configs)
            {
                bool ipenable = (bool)item["IPEnabled"];

                if (ipenable)
                {
                    if (item["MACAddress"].ToString() == macAddress)
                    {
                        var managementobject = item as ManagementObject;

                        string[] dns = new string[2];
                        dns[0] = primaryDns;
                        dns[1] = secondaryDns;

                        var newdns = managementobject.GetMethodParameters("SetDNSServerSearchOrder");
                        newdns["DNSServerSearchOrder"] = dns;
                        managementobject.InvokeMethod("SetDNSServerSearchOrder", newdns, null);
                    }
                }
            }
        }

        /// <summary>
        /// set the network as dynamic dns enable. 
        /// </summary>
        /// <param name="macAddress"></param>
        public static void SetDynamicDns(string macAddress)
        {
            var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration");

            var configs = networkConfigMng.GetInstances();

            foreach (var item in configs)
            {
                bool ipenable = (bool)item["IPEnabled"];

                if (ipenable)
                {
                    if (item["MACAddress"].ToString() == macAddress)
                    {
                        var managementobject = item as ManagementObject;

                        var mboDNS = managementobject.GetMethodParameters("SetDNSServerSearchOrder");
                        if (mboDNS != null)
                        {
                            mboDNS["DNSServerSearchOrder"] = null;
                            managementobject.InvokeMethod("SetDNSServerSearchOrder", mboDNS, null);
                        }

                    }
                }
            }

        }

        public static List<NetworkSetting> GetActiveNetworks()
        {
            List<NetworkSetting> networks = new List<NetworkSetting>();

            var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration");

            var configs = networkConfigMng.GetInstances();

            foreach (var item in configs)
            {
                bool ipenable = (bool)item["IPEnabled"];

                if (ipenable)
                {
                    NetworkSetting network = new NetworkSetting
                    {
                        MACAddress = item["MACAddress"].ToString(),
                        SettingID = item["SettingID"].ToString()
                    };


                    if (string.IsNullOrEmpty(network.MACAddress))
                    {
                        continue;
                    }

                    string[] dnsServers = item["DNSServerSearchOrder"] as string[];

                    if (dnsServers != null && dnsServers.Any())
                    {
                        string serverone = dnsServers[0];

                        if (!string.IsNullOrEmpty(serverone) && serverone.Length > 6)
                        {
                            network.PrimaryDnsServer = serverone.Trim();
                        }

                        if (dnsServers.Count() > 1)
                        {
                            string servertwo = dnsServers[1];

                            if (!string.IsNullOrEmpty(servertwo) && servertwo.Length > 6)
                            {
                                network.SecondDnsServer = servertwo.Trim();
                            }
                        }
                    }

                    network.AutomaticDns = isAutomaticDns(network.SettingID);
                      
                    networks.Add(network);
                }

            }

            return networks;
        }

        /// <summary>
        /// this method only used for unit testing.
        /// </summary>
        /// <param name="settingGuid"></param>
        /// <returns></returns>
        public static NetworkSetting GetNetworkSettingBySettingId(Guid settingGuid)
        {
            var list = GetActiveNetworks();

            foreach (var item in list)
            {
                if (item.SettingGuid == settingGuid)
                {
                    return item;
                }
            }
            return null;
        }

        public static NetworkSetting GetNetworkSettingByMacAddress(string macAddress)
        {
            var list = GetActiveNetworks();

            foreach (var item in list)
            {
                if (item.MACAddress == macAddress)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// check this network use dynamic DNS or static DNS. dynamic dns obtain dns automaically. 
        /// </summary>
        /// <param name="NetworkSettingId"></param>
        /// <returns></returns>
        private static bool isAutomaticDns(string NetworkSettingId)
        {

            var servers = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + NetworkSettingId, "NameServer", null) as string;

            if (string.IsNullOrEmpty(servers) || servers.Length < 7)
            {
                return true;
            }
            else
            {
                return false;
            }


        }


        public static void BackupNetworkSetting()
        {
            //var networks = NetworkUtility.GetActiveNetworks();

            //NetworkSettingStore.UpdateBackup(networks);

            //foreach (var item in networks)
            //{
            //    // set the local dns server to this instance. 
            //    if (!string.IsNullOrEmpty(item.PrimaryDnsServer) && item.PrimaryDnsServer != DnsConstants.localIp.ToString())
            //    {
            //        NetworkUtility.SetDnsServer(item.MACAddress, DnsConstants.localIp.ToString(), item.PrimaryDnsServer);
            //    }
            //}
        }

        public static void RestoreNetworkSetting()
        {
            //var networks = NetworkUtility.GetActiveNetworks();

            //foreach (var item in networks)
            //{
            //    var oldsetting = NetworkSettingStore.Get(item.Id);
            //    if (oldsetting != null)
            //    {
            //        if (oldsetting.AutomaticDns)
            //        {
            //            NetworkUtility.SetDynamicDns(item.MACAddress);
            //        }
            //        else
            //        {
            //            NetworkUtility.SetDnsServer(item.MACAddress, oldsetting.PrimaryDnsServer, oldsetting.SecondDnsServer);
            //        }
            //    }
            //}

        }


    }
}
