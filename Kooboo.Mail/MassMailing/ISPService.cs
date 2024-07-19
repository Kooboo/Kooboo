using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Mail.MassMailing.Model;

namespace Kooboo.Mail.MassMailing
{
    public class ISPService
    {
        public static ISPService Instance { get; set; } = new ISPService();

        public async Task<ISP> DetectISP(string RCPTTO)
        {
            RCPTTO = RCPTTO.ToLower();

            var EmailDomain = Utility.AddressUtility.GetEmailDomain(RCPTTO);

            var MX = await MxRecordProvider.GetMx(EmailDomain);

            return DetectISPByMx(MX.Select(o => o.Domain).ToList());
        }


        public ISP DetectISPByMx(List<string> MXs)
        {
            if (MXs != null)
            {
                foreach (var item in MXs)
                {
                    var root = GetRootDomain(item);

                    if (root != null)
                    {
                        var ISPByMx = ISPContainer.instance.DictList.Values.ToList().Find(o => o.MxDomains.Contains(root));

                        if (ISPByMx != null)
                        {
                            return ISPByMx;
                        }
                    }
                }
            }
            return NewISP(MXs.FirstOrDefault());
        }

        private ISP NewISP(string mx)
        {
            if (ISPContainer.instance.DictList.ContainsKey(mx))
            {
                return ISPContainer.instance.DictList[mx];
            }
            else
            {
                var isp = new ISP() { Name = mx };
                ISPContainer.instance.DictList[mx] = isp;
                ISPContainer.instance.SaveDisk(isp);
                return isp;
            }
        }

        public ISP GetByName(string ISPName)
        {
            return ISPContainer.instance.Get(ISPName);
        }


        private string GetRootDomain(string fullDomain)
        {
            var result = Kooboo.Lib.Domain.DomainService.Parse(fullDomain);

            if (result != null)
            {
                return result.Root;
            }
            return null;
        }

    }
}
