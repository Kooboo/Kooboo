//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Kooboo.Lib.Helper
{
    public static class NetworkHelper
    {
        public static string GetLocalIpAddress()
        {
            //  return "127.0.0.1";

            List<string> IpAdds = new List<string>();

            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
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
            var definePort = Lib.AppSettingsUtility.GetInt("port");

            if (definePort > 0)
            {
                return false;
            }


            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (var item in ipEndPoints)
            {
                if (item.Port == port)
                {
                    return true;

                    // if in use. 
                    //bool IsInUsed = false;
                    //try
                    //{
                    //    TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Any, port);
                    //    tcpListener.Start();
                    //    tcpListener.Stop();
                    //    tcpListener = null;
                    //}
                    //catch (SocketException ex)
                    //{
                    //    IsInUsed = true;
                    //}
                    //finally
                    //{

                    //}
                    //return IsInUsed;
                }
            }

            return false;
        }




        public static long Ping(string destIp)
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(destIp, 5000);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    return reply.RoundtripTime;
                }
            }
            catch (Exception)
            {
            }


            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(destIp, 5000);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    return reply.RoundtripTime;
                }
            }
            catch (Exception)
            {
            }

            return long.MaxValue;
        }

        public static long Ping(System.Net.IPAddress destIp)
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(destIp, 2000);
                if (reply != null && reply.RoundtripTime > 0)
                {
                    return reply.RoundtripTime;
                }
            }
            catch (Exception)
            {

            }
            return long.MaxValue;
        }

        public static System.Net.IPAddress PingAddress(string host)
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(host, 5000);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    return reply.Address;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
