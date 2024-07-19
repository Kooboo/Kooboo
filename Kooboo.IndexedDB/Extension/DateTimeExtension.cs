//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB
{
    public static class DateTimeExtension
    {
        public static long ToInt64(this DateTime date)
        {
            return date.Ticks;

            //TimeSpan last = date.ToUniversalTime() - GlobalSettings.UTCStartdate;
            //return Convert.ToInt64(Math.Floor(last.TotalMilliseconds));
        }
        /// <summary>
        /// use ID to represent a day, this is used for day key.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromInt64(this DateTime date, long datetick)
        {
            return new DateTime(datetick);
            //return GlobalSettings.UTCStartdate.AddMilliseconds(datelong);
            // return GlobalSettings.UTCStartdate.AddTicks(datelong);
        }

        /// <summary>
        /// use ID to represent a day, this is used for day key.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int DayToInt(this DateTime date)
        {
            TimeSpan last = date - GlobalSettings.UTCStartdate;
            return (int)last.TotalDays;
        }

    }
}
