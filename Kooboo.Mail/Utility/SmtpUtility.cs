//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading.Tasks; 
using DNS.Client;
using DNS.Protocol.ResourceRecords;

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
                  
        
        public static async Task<SendSetting> GetSendSetting(Data.Models.ServerSetting serverSetting, bool IsOnlineServer,  string MailFrom, string RcptTo)
        {
          
            SendSetting setting = new SendSetting();

            if (IsOnlineServer && !serverSetting.CanDirectSendEmail)
            { 
                setting.UseKooboo = true;
                setting.KoobooServerIp = serverSetting.SmtpServerIP;
                setting.Port = serverSetting.SmtpPort;
                setting.HostName = System.Net.Dns.GetHostName();
                setting.LocalIp = System.Net.IPAddress.Any;
                setting.OkToSend = true;  
            }
           else
            { 
                var mxs = await Kooboo.Mail.Utility.SmtpUtility.GetMxRecords(RcptTo); 
                if (mxs == null || mxs.Count() ==0)
                {
                    setting.OkToSend = false;
                    setting.ErrorMessage = "Mx records not found";
                }
                else
                {
                    setting.OkToSend = true;
                    setting.Mxs = mxs; 

                    if (serverSetting.ReverseDns !=null && serverSetting.ReverseDns.Count()>0)
                    {          
                        var dns = serverSetting.ReverseDns[0];  //TODO: random it.  
                        setting.LocalIp =  System.Net.IPAddress.Parse(dns.IP);
                        setting.HostName = dns.HostName; 
                    }
                    else
                    {
                        setting.LocalIp = System.Net.IPAddress.Any;
                        setting.HostName = System.Net.Dns.GetHostName(); 
                    } 
                } 
            } 
            return setting; 
        }

       
         
    }


    public class DnsLookup
    {
        public static TimeSpan MXCacheExpire = TimeSpan.FromHours(1);
        private static object mxResolveLock = new object();

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
            var mxsItem = MemoryCache.Default.Get(key) as CacheItem;
            if (mxsItem == null)
            {
                var mxs = await ResolveMX(domainName);
                mxsItem = new CacheItem
                {
                    MXs = mxs
                };
                MemoryCache.Default.Add(key, mxsItem, new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.UtcNow.Add(MXCacheExpire),
                });
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
        }
    }


    public class SendSetting
    {
        public bool  OkToSend { get; set; }

        public string ErrorMessage { get; set; }

        public string HostName { get; set; }

        public System.Net.IPAddress LocalIp { get; set; }

        public bool UseKooboo { get; set; }

        public int Port { get; set; } = 25; 

        public string KoobooServerIp { get; set; }

        public List<string> Mxs { get; set; }
    }
}
