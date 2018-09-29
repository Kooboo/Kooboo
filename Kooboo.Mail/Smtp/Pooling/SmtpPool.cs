//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Smtp
{
    /// <summary>
    /// Todo:
    /// - Allow concurrent connections for ip -> domain
    /// - Control max allowed transactions in one connection.
    /// - Control allowed connection time out
    /// </summary>
    public class SmtpPool
    {
        static SmtpPool()
        {
            Instance = new SmtpPool();
        }

        public static SmtpPool Instance { get; set; }

        private ConcurrentDictionary<string, ConcurrentQueue<SmtpPoolItem>> _pool;

        public SmtpPool()
        {
            _pool = new ConcurrentDictionary<string, ConcurrentQueue<SmtpPoolItem>>();
        }

        public async Task<SmtpPoolItem> AcquireAsync(string ip, string host, Func<Task<SmtpPoolItem>> newItem)
        {
            var hostPool = EnsureHostPool(ip, host);

            SmtpPoolItem result;
            if (hostPool.TryDequeue(out result) && result.Client.Connected)
            {
                try
                {
                    await result.Client.Rset();
                    return result;
                }
                catch
                {
                }
            }

            result = await newItem();
            if (result == null)
                return null;

            result.IP = ip;
            result.Host = host;

            return result;
        }

        public async Task ReleaseAsync(SmtpPoolItem item)
        {
            item.SentMails++;
            if (item.AllowedMails > 0 && item.SentMails >= item.AllowedMails)
            {
                // Over allowed transactions, close the connection
                if (item.Client.Connected)
                {
                    await item.Client.Quit();
                }
                item.Client.Dispose();
            }
            else
            {
                var hostPool = EnsureHostPool(item.IP, item.Host);
                hostPool.Enqueue(item);
            }

        }

        private ConcurrentQueue<SmtpPoolItem> EnsureHostPool(string ip, string host)
        {
            // IP might be null
            var key = ip + "_" + host;
            return _pool.GetOrAdd(key, k => new ConcurrentQueue<SmtpPoolItem>());
        }
    }
}
