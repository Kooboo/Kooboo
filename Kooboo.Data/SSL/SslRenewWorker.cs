using Kooboo.Data.Interface;
using System;
using System.Linq;

namespace Kooboo.Data.SSL
{
    public class SslRenewWorker : IBackgroundWorker
    {
        public static int lastCheckDay;

        public int Interval => 60 * 60;

        public DateTime LastExecute { get; set; }

        public void Execute()
        {
            Log.Instance.Ssl.Write("try renewing at: " + DateTime.Now.ToLongTimeString());

            var todayInt = DateTime.Now.DayOfYear;
            if (todayInt != lastCheckDay)
            {
                lastCheckDay = todayInt;

                var list = GlobalDb.SslCertificate.GetAllInUsed();

                var checkbefore = DateTime.Now.AddDays(10);

                var items = list.Where(o => o.Expiration < checkbefore).ToList();

                var logitems = items.Select(o => o.Domain).ToList();

                if (logitems != null && logitems.Any())
                {
                    Kooboo.Data.Log.Instance.Ssl.WriteObj(logitems);
                }

                foreach (var item in items)
                { 
                    SslService.SetSsl(item.Domain, item.OrganizationId); 
                }
            }
        }
    }
}
