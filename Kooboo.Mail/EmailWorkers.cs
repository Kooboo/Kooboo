//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Tasks;

namespace Kooboo.Mail
{
    public class EmailWorkers
    {
        static EmailWorkers()
        {
            Workers = new List<IWorkerStarter>();
            Workers.Add(new Smtp.SmtpServer("Receive"));

            var imapServer = new Imap.ImapServer(143);
            Workers.Add(imapServer);

            //if (Data.AppSettings.IsOnlineServer)
            //{
            var dbcert = Kooboo.Data.SSL.SslCertificateProvider.SelectCertificate2(Settings.ImapDomain);

            if (dbcert != null)
            {
                var sslimapServer = new Imap.ImapServer(993, Imap.SslMode.SSL);
                Workers.Add(sslimapServer);
                // var sslSmtp = new Kooboo.Mail.Smtp.SmtpServer("sslreceive", 465, dbcert);
                // Workers.Add(sslSmtp);
            }
            // }
            Workers.Add(new Smtp.SmtpServer("Relay", 587));
        }

        public static List<IWorkerStarter> Workers { get; private set; }

        public static void Start()
        {
            foreach (var each in Workers)
            {
                each.Start();
            }
            Mail.Queue.MailQueueWorker.Instance.Start();
        }

        public static void Stop()
        {
            foreach (var each in Workers)
            {
                each.Stop();
            }
        }
    }
}
