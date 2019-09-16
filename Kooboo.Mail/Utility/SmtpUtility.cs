//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading.Tasks; 
using DNS.Client;
using DNS.Protocol.ResourceRecords;
using System.Configuration;

namespace Kooboo.Mail.Utility
{
   public static class SmtpUtility
    {  

        public static string GetString(byte[] data)
        {
            var encodingresult = Lib.Helper.EncodingDetector.GetEmailEncoding(data);

            System.Text.Encoding encoding = null;
            if (encodingresult != null && !string.IsNullOrEmpty(encodingresult.Charset))
            {
                encoding = System.Text.Encoding.GetEncoding(encodingresult.Charset);
            }

            if (encoding == null)
            {
                encoding = System.Text.Encoding.UTF8;
            }

            string text = encoding.GetString(data);

            if (text != null && encodingresult != null && !string.IsNullOrWhiteSpace(encodingresult.CharSetText))
            {
                text = text.Replace(encodingresult.CharSetText, "charset=utf8");
            }

            return text;
        }

        public static async Task<List<string>> GetMxRecords(string RcptTo)
        {
            string To = Utility.AddressUtility.GetAddress(RcptTo); 
            int index = To.IndexOf("@");  
            if (index >-1)
            {
                string domain = To.Substring(index + 1); 
                var mxs = await DnsLookup.ResolveCachedMX(domain); 
                return mxs.ToList(); 
            } 
            return null; 
        }
                 
    }


    public class DnsLookup
    {
        public static TimeSpan MXCacheExpire = TimeSpan.FromHours(1);
        private static object mxResolveLock = new object();

        private static Dictionary<string, CacheItem> DefaultCache = new Dictionary<string, CacheItem>();

        static DnsLookup()
        {
            var localMtaDns = System.Configuration.ConfigurationManager.AppSettings["localMtaDns"];
            if (!String.IsNullOrEmpty(localMtaDns))
            {
                Client = new DnsClient(localMtaDns);
            }
            else
            {
                var localDns = GetLocalDns();
                if (localDns == null)
                    throw new Exception("Can not find local DNS, specify extra DNS in app.config");

                Client = new DnsClient(localDns);
            }
        }

        public static DnsClient Client { get; set; }

        public static async Task<string[]> ResolveCachedMX(string domainName)
        {
            var key = "MXCache_" + domainName;
            CacheItem mxsItem = null;
            if(DefaultCache.ContainsKey(key))
            {
                mxsItem = DefaultCache[key];
            }

            if (mxsItem == null || mxsItem.CacheTime.Add(MXCacheExpire)>DateTime.UtcNow)
            {
                var mxs = await ResolveMX(domainName);
                
                lock (mxResolveLock)
                {
                    if (mxsItem == null || mxsItem.CacheTime.Add(MXCacheExpire) > DateTime.UtcNow)
                    {
                        mxsItem = new CacheItem
                        {
                            MXs = mxs,
                            CacheTime = DateTime.UtcNow
                        };
                        if (DefaultCache.ContainsKey(key))
                        {
                            DefaultCache[key] = mxsItem;
                        }
                        else
                        {
                            DefaultCache.Add(key, mxsItem);
                        }
                    }
                        
                }
                return mxs;
            }
            return mxsItem.MXs;
        }

        public static async Task<string[]> ResolveMX(string domainName)
        {
            string[] result = null;
            for (int i = 0; i < 3 && result == null; i++)
            {
                try
                {
                    result = await ResolveMXOnce(domainName);
                    if (result.Length > 0)
                        return result;
                }
                catch (ResponseException)
                {
                    // Get result from server but failed result
                    return new string[0];
                }
                catch (OperationCanceledException)
                {
                    // Retry when connection timeout
                }
                catch
                {
                    // Unknown exception
                    return new string[0];
                }
            }
            return result;
        }

        public static async Task<string[]> ResolveMXOnce(string domainName)
        {
            var response = await Client.Resolve(domainName, DNS.Protocol.RecordType.MX);

            var result = response.AnswerRecords
                .Where(r => r.Type == DNS.Protocol.RecordType.MX)
                .Cast<MailExchangeResourceRecord>()
                .OrderBy(o => o.Preference)
                .Select(o => o.ExchangeDomainName.ToString())
                .ToArray();

            return result;
        }

        private static IPAddress GetLocalDns()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (IPAddress ip in nic.GetIPProperties().DnsAddresses)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip;
                        }
                    }
                }
            }

            return null;
        }

        class CacheItem
        {
            public string[] MXs { get; set; }

            public DateTime CacheTime { get; set; }
        }
    }
     
}
