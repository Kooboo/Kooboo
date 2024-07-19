using System;
using Kooboo.Data;
using Kooboo.Data.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Mail.Transport
{
    public class MailDomainCheck
    {
        public static MailDomainCheck Instance { get; set; } = new MailDomainCheck();

        public MemoryCache DomainCache = new MemoryCache(new MemoryCacheOptions() { });

        public Domain GetByEmailAddress(string emailaddress)
        {
            var host = Kooboo.Mail.Utility.AddressUtility.GetEmailDomain(emailaddress);

            if (host == null)
            {
                return null;
            }
            host = host.ToLower();

            return GetByHost(host);

        }

        public Domain GetByHost(string host)
        {
            if (!Data.AppSettings.IsOnlineServer)
            {
                return GlobalDb.Domains.Get(host);
            }

            if (DomainCache.TryGetValue(host, out Domain result))
            {
                return result;
            }

            // get from remote. 
            var url = Kooboo.Data.Helper.AccountUrlHelper.Domain("DomainForMailServer");
            url += "?domain=" + host;

#if DEBUG
            url += "&IP=43.136.60.242";
#endif

            Domain domain = null;

            try
            {
                domain = Lib.Helper.HttpHelper.Get<Domain>(url);
            }
            catch (Exception)
            {
            }

            if (domain != null)
            {
                DomainCache.Set<Domain>(host, domain, TimeSpan.FromDays(2));
            }
            else
            {
                DomainCache.Set<Domain>(host, domain, TimeSpan.FromHours(1));
            }

            return domain;
        }


        // Below from account. 
        //public Domain DomainForMailServer(string domain, ApiCall call)
        //{
        //    return Service.DomainService.Instance.GetMailServerDomain(call.Context.Request.IP, domain);
        //}

    }
}
