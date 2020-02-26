using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    internal static class EpochTime
    {
        private static DateTime epochStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertEpochToDateTime(long seconds)
        {
            return epochStartDateTime.AddSeconds(seconds);
        }

        public static long ConvertDateTimeToEpoch(this DateTime datetime)
        {
            if (datetime < epochStartDateTime)
            {
                return 0;
            }

            return Convert.ToInt64((datetime.ToUniversalTime() - epochStartDateTime).TotalSeconds);
        }
    }
}
