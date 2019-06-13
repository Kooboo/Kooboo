//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
    public static class IPHelper
    {

        static IPHelper()
        {
            InternalIpPrefix = new List<string>();
            InternalIpPrefix.Add("127.");
            InternalIpPrefix.Add("10.");
            InternalIpPrefix.Add("169.254.");
            InternalIpPrefix.Add("192.0.2.");
            InternalIpPrefix.Add("192.88.99.");
            InternalIpPrefix.Add("192.168.");
            InternalIpPrefix.Add("198.51.100");
        }

        private static List<string> InternalIpPrefix { get; set; }

        private static long part1 { get; set; } = 256 * 256 * 256;
        private static long part2 { get; set; } = 256 * 256;
        private static long part3 { get; set; } = 256; 

        public static int ToInt(string ip)
        {
            string[] segs = ip.Split('.');
            if (segs != null && segs.Length == 4)
            {
                long addvalue = 0;
                long one = long.Parse(segs[0]) * part1;
                long two = long.Parse(segs[1]) * part2;
                long three = long.Parse(segs[2]) * part3;
                long four = long.Parse(segs[3]);
                addvalue = one + two + three + four;
                var value = addvalue + int.MinValue;  
                return (int)value;  
            }
            return int.MinValue;  
        }
        
        public static string FromInt(int value)
        {
           long orgvalue = (long)value - (long)int.MinValue;

            long onevalue = orgvalue % part1; 
            long one = (orgvalue - onevalue) / part1;

            long twovalue = onevalue % part2; 
            long two = (onevalue - twovalue) / part2;

            long threevalue = twovalue % part3;

            long three = (twovalue - threevalue) / part3;

            return one.ToString() + "." + two.ToString() + "." + three.ToString() + "." + threevalue;  
        }
  
        public static int CompareIp(string ipx, string ipy)
        { 
           return  ToInt(ipx) -ToInt(ipy);  
        } 

        public static bool IsIP(string input)
        {
            System.Net.IPAddress ipout;
            return System.Net.IPAddress.TryParse(input, out ipout); 
        }

        public static bool IsInSameCClass(string ipx, string ipy)
        {
            if (ipx == null || ipx == null)
            {
                return false; 
            }

            string[] x = ipx.Split('.');
            string[] y = ipy.Split('.'); 
           if (x == null || x.Length !=4)
            {
                return false; 
            }

           if (y == null || y.Length !=4)
            {
                return false; 
            }

           if (x[0] == y[0] && x[1] == y[1]  && x[2] == y[2])
            {
                return true; 
            }
            return false; 
        }
         
        public static bool IsLocalIp(string Ip)
        {  
            //TODO: improve it.  
            foreach (var item in InternalIpPrefix)
            {
                if (Ip.StartsWith(item))
                {
                    return true;
                }
            }
            return false; 
        } 
    }
}


//https://en.wikipedia.org/wiki/Reserved_IP_addresses


//IPv4[edit]
//See also: IPv4 § Special-use addresses
//Address block(CIDR)    Range Number of addresses Scope Purpose
//0.0.0.0/8	0.0.0.0 –
//0.255.255.255	16,777,216	Software Used for broadcast messages to the current("this")[1]
//10.0.0.0/8	10.0.0.0 –
//10.255.255.255	16,777,216	Private network Used for local communications within a private network[2]
//100.64.0.0/10	100.64.0.0 –
//100.127.255.255	4,194,304	Private network Used for communications between a service provider and its subscribers when using a carrier-grade NAT[3]
//127.0.0.0/8	127.0.0.0 –
//127.255.255.255	16,777,216	Host Used for loopback addresses to the local host[4]
//169.254.0.0/16	169.254.0.0 –
//169.254.255.255	65,536	Subnet Used for link-local addresses between two hosts on a single link when no IP address is otherwise specified, such as would have normally been retrieved from a DHCP server[5]
//172.16.0.0/12	172.16.0.0 –
//172.31.255.255	1,048,576	Private network Used for local communications within a private network[2]
//192.0.0.0/24	192.0.0.0 –
//192.0.0.255	256	Private network Used for the IANA IPv4 Special Purpose Address Registry[6]
//192.0.2.0/24	192.0.2.0 –
//192.0.2.255	256	Documentation Assigned as "TEST-NET-1" for use in documentation and examples.It should not be used publicly.[7]
//192.88.99.0/24	192.88.99.0 –
//192.88.99.255	256	Internet Used by 6to4 anycast relays (deprecated)[8]
//192.168.0.0/16	192.168.0.0 –
//192.168.255.255	65,536	Private network Used for local communications within a private network[2]
//198.18.0.0/15	198.18.0.0 –
//198.19.255.255	131,072	Private network Used for testing of inter-network communications between two separate subnets[9]
//198.51.100.0/24	198.51.100.0 –
//198.51.100.255	256	Documentation Assigned as "TEST-NET-2" for use in documentation and examples.It should not be used publicly.[7]
//203.0.113.0/24	203.0.113.0 –
//203.0.113.255	256	Documentation Assigned as "TEST-NET-3" for use in documentation and examples.It should not be used publicly.[7]
//224.0.0.0/4	224.0.0.0 –
//239.255.255.255	268,435,456	Internet Reserved for multicast[10]
//240.0.0.0/4	240.0.0.0 –
//255.255.255.254	268,435,456	Internet Reserved for future use[11]
//255.255.255.255/32	255.255.255.255	1	Subnet Reserved for the "limited broadcast" destination address[11]