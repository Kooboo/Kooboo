using System;
using System.Net;
using Kooboo.Lib.Extensions;


namespace Kooboo.Lib.Utilities.UAParser
{
    public static class GoogleBotVerifier
    {

        public static bool IsGoogleBot(ClientInfo client, string IPAddress)
        {
            if (client.Application == null || !client.Application.IsWebBrowser || client.Application.Name != "Google Bot")
            {
                return false;
            }

            if (!IsGoogleIP(IPAddress))
            {
                return false;
            }

            return VerifyByRDNS(IPAddress);
        }


        public static bool IsGoogleIP(string VerifyIP)
        {
            // TODO: See: https://developers.google.com/search/docs/crawling-indexing/verifying-googlebot

            if (GoogleIPList == null)
                return true;

            foreach (var item in GoogleIPList.Prefixes)
            {
                // Because the Google IP list is given in CIDR format, you need to determine whether the IP is in CIDR format range
                if (IsIPInCIDRRange(VerifyIP, item.ToString()))
                    return true;
            }
            return false;
        }

        // because IP is not always up to date, can use RDNS for a second check, RDNS can not be fake. 
        public static bool VerifyByRDNS(string IP)
        {
            if (System.Net.IPAddress.TryParse(IP, out var ipaddress))
            {
                var RDNS = Kooboo.Lib.DnsRequest.RequestManager.GetReverseDns(ipaddress);

                if (RDNS != null && RDNS.Trim().ToLower().EndsWith(".googlebot.com"))
                {
                    return true;
                }

                return false;

            }
            return false;
        }

        private static object _locker = new object();

        private static DateTime LastCheck = DateTime.Now;
        private static IPList _googleIpList;
        private static IPList GoogleIPList
        {
            get
            {

                if (LastCheck < DateTime.Now.AddDays(-3))
                {
                    _googleIpList = null;
                }

                if (_googleIpList == null)
                {
                    lock (_locker)
                    {
                        if (_googleIpList == null)
                        {
                            var IPReader = new GoogleIPList();
                            var list = IPReader.ReadAllLines();
                            _googleIpList = list;
                            LastCheck = DateTime.Now;
                        }
                    }
                }

                return _googleIpList;
            }
        }


        private static bool IsIPInCIDRRange(string ipAddress, string cidr)
        {
            try
            {
                // Convert the IP address and CIDR to the corresponding object
                IPAddress ip = IPAddress.Parse(ipAddress);
                string[] cidrParts = cidr.Split('/');
                IPAddress cidrIP = IPAddress.Parse(cidrParts[0]);
                int subnetMaskLength = int.Parse(cidrParts[1]);

                // Use the IsInRange method of IPAddressExtensions to check whether they are in the CIDR range
                return ip.IsInRange(cidrIP, subnetMaskLength);
            }
            catch (Exception)
            {
                // Handling exceptions (such as invalid IP addresses or CIDR ranges)
                // output Logger
                // Console.WriteLine(ex.ToString());
                return false;
            }
        }


    }
}


/*
 * 
 * https://developers.google.com/search/docs/crawling-indexing/verifying-googlebot
 * 
 * Verifying Googlebot and other Google crawlers

bookmark_border
You can verify if a web crawler accessing your server really is a Google crawler, such as Googlebot. This is useful if you're concerned that spammers or other troublemakers are accessing your site while claiming to be Googlebot.

Google's crawlers fall into three categories:

Type	Description	Reverse DNS mask	IP ranges
Googlebot	The main crawler for Google's search products. Always respects robots.txt rules.	crawl-***-***-***-***.googlebot.com or geo-crawl-***-***-***-***.geo.googlebot.com	googlebot.json
Special-case crawlers	Crawlers that perform specific functions (such as AdsBot), which may or may not respect robots.txt rules.	rate-limited-proxy-***-***-***-***.google.com	special-crawlers.json
User-triggered fetchers	Tools and product functions where the end user triggers a fetch. For example, Google Site Verifier acts on the request of a user. Because the fetch was requested by a user, these fetchers ignore robots.txt rules.	***-***-***-***.gae.googleusercontent.com	user-triggered-fetchers.json
There are two methods for verifying Google's crawlers:

Manually: For one-off lookups, use command line tools. This method is sufficient for most use cases.
Automatically: For large scale lookups, use an automatic solution to match a crawler's IP address against the list of published Googlebot IP addresses.
 * 
 * 
 * 
 * 
 */