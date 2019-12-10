using Kooboo.Data.Interface;
using System;

namespace Kooboo.Data.SSL
{
    public class SslRenewWorker : IBackgroundWorker
    {
        public int Interval => 60 * 50 * 2;

        public DateTime LastExecute { get; set; }

        public void Execute()
        {   
            if (DateTime.Now.Hour == 2 || DateTime.Now.Hour == 3)
            {
                foreach (var item in Kooboo.Data.GlobalDb.SslCertificate.All())
                {
                    if (item.Expiration < DateTime.Now.AddDays(30))
                    {
                        // Renew the certificate...
                         SslService.SetSsl(item.Domain, item.OrganizationId);  
                    }
                }
            }
        }
    }
}
