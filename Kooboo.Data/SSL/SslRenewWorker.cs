using Kooboo.Data.Interface;
using System;

namespace Kooboo.Data.SSL
{
    public class SslRenewWorker : IBackgroundWorker
    {
        public static int lastCheckDay;

        public int Interval => 60 * 60;

        public DateTime LastExecute { get; set; }

        public void Execute()
        {
            var now = DateTime.Now;
             
            var todayInt = now.DayOfYear;
            if (todayInt != lastCheckDay)
            {
                if (now.Hour == 2 || now.Hour == 3 || now.Hour == 4 || now.Hour == 5 || now.Hour == 6)
                {
                    foreach (var item in Kooboo.Data.GlobalDb.SslCertificate.GetAllInUsed())
                    {
                        if (item.Expiration < DateTime.Now.AddDays(10))
                        {
                            SslService.SetSsl(item.Domain, item.OrganizationId);
                        }
                    }

                    lastCheckDay = todayInt;
                }

            }
        }
    }
}
