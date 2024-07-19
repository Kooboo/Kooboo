using System.Linq;
using Kooboo.Api;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class MailReportApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "mailreport";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }


        public IEnumerable<Mail.Models.DmarcReport> DmarcList(ApiCall call)
        {

            if (EmailForwardManager.RequireForward(call.Context))
            {
                return EmailForwardManager.Get<List<Mail.Models.DmarcReport>>(this.ModelName, nameof(MailReportApi.DmarcList), call.Context.User);
            }

            var user = call.Context.User;
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(user.CurrentOrgId);
            var list = orgdb.Dmarc.Last();

            if (list != null)
            {
                return list.ToList();
            }

            return list;
        }
    }
}
