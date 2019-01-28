//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
   public static class NetworkHelper
    {
        public static string GetLocalIpAddress()
        {
            List<string> IpAdds = new List<string>();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IpAdds.Add(ip.ToString());
                }
            }

            foreach (var item in IpAdds)
            {
                if (item != "127.0.0.1" && !item.StartsWith("192."))
                {
                    return item;
                }
            }

            if (IpAdds.Count() == 0)
            {
                return null;
            }

            return IpAdds.First();
        }

        public static bool IsPortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (var item in ipEndPoints)
            {
                if (item.Port == port) 
                {
                    // if in use. 
                    bool IsInUsed = false; 
                    try
                    {
                        TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Any,  port);
                        tcpListener.Start();
                        tcpListener.Stop();
                        tcpListener = null; 
                    }
                    catch (SocketException ex)
                    {
                        IsInUsed = true; 
                    }
                    finally
                    {

                    }
                    return IsInUsed; 
                }
            }
               
            return false;
        }
    }
}
