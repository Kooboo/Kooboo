using System;
using System.Threading.Tasks;
using Kooboo.Mail.Smtp;

namespace Kooboo.Mail.Spam
{
    public class SessionVerifier
    {

        public static SessionVerifier Instance { get; set; } = new SessionVerifier();

        public async Task<SessionResult> ValidateSession(SmtpSession session)
        {
            // valie RDNS. 
            SessionResult result = new SessionResult();
            try
            {
                result.RDNS = await RDNSVerifier.IsValidRDNS(session.ClientHostName, session.RemoteIp);
                string mailfrom = GetMailFrom(session);
                result.SPF = await SpfVerifier.Validate(mailfrom, session.RemoteIp);
                result.DKIM = new KDkimVerifier().Verify(session.MessageBody);

                Kooboo.Data.Log.Instance.Email.Write("DKIM Result " + result.DKIM.ToString() + "\r\n------------\r\n");
                Kooboo.Data.Log.Instance.Email.Write(session.MessageBody);
                Kooboo.Data.Log.Instance.Email.Write("\r\n-----------");
            }
            catch (Exception)
            {

            }

            var Json = System.Text.Json.JsonSerializer.Serialize(result);
            Kooboo.Data.Log.Instance.EmailDebug.Write(Json);

            return result;
        }

        //Copy from incoming.
        internal static string GetMailFrom(Smtp.SmtpSession session)
        {
            foreach (var item in session.Log)
            {
                if (item.Key.Name == Smtp.SmtpCommandName.MAILFROM && item.Value.Code == 250)
                {
                    return Utility.SmtpUtility.GetEmailFromMailFromLine(item.Key.Value);
                }
            }
            return null;
        }

    }
}
