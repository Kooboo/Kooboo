using Kooboo.Api;
using Kooboo.Mail.Models;
using Kooboo.Mail.Transport;


namespace Kooboo.Web.Api.Implementation.Mails
{
    public class MailReportInApi : IApi
    {
        public string ModelName => "MailReportIn";
        public bool RequireSite => false;
        public bool RequireUser => false;

        public bool Smtp(SmtpReportIn item, ApiCall call)
        {
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(item.HeaderFrom);
            if (orgDb != null)
            {
                var address = orgDb.Email.Find(item.HeaderFrom);
                if (address != null)
                {
                    var mailDb = Kooboo.Mail.Factory.DBFactory.UserMailDb(address.UserId, orgDb.OrganizationId);
                    mailDb.SmtpDelivery.AddReport(item);
                    return true;
                }
            }

            return true;
        }


        public bool Dmarc(DmarcReport item, ApiCall call)
        {
            string from = item.MsgFrom;
            if (from == null)
            {
                from = item.MailFrom;
            }

            if (from == null)
            {
                from = item.OurDomain;
            }

            if (from != null)
            {
                Kooboo.Data.Models.Domain domain = null;

                if (from.StartsWith("@"))
                {
                    from = from.Substring(1);
                }

                if (from.IndexOf("@") > 0)
                {
                    domain = MailDomainCheck.Instance.GetByEmailAddress(from);
                }
                else
                {
                    domain = MailDomainCheck.Instance.GetByHost(from);
                }

                if (domain != null)
                {
                    var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(domain.OrganizationId);
                    if (orgdb != null)
                    {
                        orgdb.Dmarc.Add(item);
                        return true;
                    }
                }
                return false;

            }
            return true;  // return true so that the other side can skip the record and move next.
        }

    }


}
