//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Kooboo.Lib.Helper
{
    public static class DateTimeHelper
    {

        public static string ParseDateFormat(List<string> DateStrings)
        {
            HashSet<string> formates = GetPossibleFormats(DateStrings);

            return ParseDateFormat(DateStrings, formates);
        }

        private static HashSet<string> GetPossibleFormats(List<string> DateStrings)
        {
            var seps = getContainsSeps(DateStrings);
            var exclus = getExclusSeps(DateStrings);

            var formates = GetPossibleFormates(seps, exclus);

            List<string> withoutyear = new List<string>();

            foreach (var item in formates)
            {
                var trimyear = TrimYear(item);
                if (!string.IsNullOrWhiteSpace(trimyear))
                {
                    withoutyear.Add(trimyear);
                }
            }

            foreach (var item in withoutyear)
            {
                formates.Add(item);
            }

            return formates;
        }

        public static string TrimYear(string formate)
        {
            int len = formate.Length;

            if (formate[0] == 'y' || formate[0] == 'Y')
            {
                for (int i = 0; i < len; i++)
                {
                    if (formate[i] == 'y' || formate[i] == 'Y')
                    {
                        continue;
                    }

                    var currentchar = formate[i];
                    if (!Lib.Helper.CharHelper.IsAscii(currentchar))
                    {
                        continue;
                    }

                    return formate.Substring(i);
                }
            }
            else if (formate[len - 1] == 'y' || formate[len - 1] == 'Y')
            {

                for (int i = len - 1; i > 0; i--)
                {
                    if (formate[i] == 'y' || formate[i] == 'Y')
                    {
                        continue;
                    }

                    var currentchar = formate[i];
                    if (!Lib.Helper.CharHelper.IsAscii(currentchar))
                    {
                        continue;
                    }

                    return formate.Substring(0, i + 1);
                }
            }

            return null;
        }

        public static string ParseDateFormat(IEnumerable<string> DateStrings, IEnumerable<string> availableFormats)
        {
            foreach (var format in availableFormats)
            {
                bool match = true;
                DateTime output;
                foreach (var date in DateStrings)
                {
                    if (!DateTime.TryParseExact(date, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out output))
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return format;
                }
            }

            return null;
        }

        private static List<string> getContainsSeps(List<string> datestrings)
        {
            List<string> result = new List<string>();
            List<string> candidates = new List<string>();

            candidates.Add(":");
            candidates.Add("/");
            candidates.Add("-");
            candidates.Add(" ");
            candidates.Add(".");
            candidates.Add(",");

            foreach (var item in candidates)
            {
                bool hascand = true;
                foreach (var date in datestrings)
                {
                    if (!date.Contains(item))
                    {
                        hascand = false;
                        break;
                    }
                }
                if (hascand)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private static List<string> getExclusSeps(List<string> datestrings)
        {
            List<string> result = new List<string>();
            List<string> candidates = new List<string>();

            candidates.Add(":");
            candidates.Add("/");
            candidates.Add("-");
            candidates.Add(" ");
            candidates.Add(".");
            candidates.Add(",");

            foreach (var item in candidates)
            {
                bool withoutcand = true;
                foreach (var date in datestrings)
                {
                    if (date.Contains(item))
                    {
                        withoutcand = false;
                        break;
                    }
                }
                if (withoutcand)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private static bool HasChar(List<string> datestrings, string sep)
        {
            foreach (var item in datestrings)
            {
                if (!item.Contains(sep))
                {
                    return false;
                }
            }
            return true;
        }

        public static HashSet<string> GetPossibleFormates(List<string> seps, List<string> exclus)
        {
            var allformats = AllFormates();

            HashSet<string> result = new HashSet<string>();
            foreach (var item in allformats)
            {
                bool match = true;

                foreach (var s in seps)
                {
                    if (!item.Contains(s))
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                {
                    continue;
                }

                // filter by exclus. 

                bool excl = false;

                foreach (var e in exclus)
                {
                    if (item.Contains(e))
                    {
                        excl = true;
                        break;
                    }
                }


                if (excl)
                {
                    continue;
                }


                result.Add(item);

            }

            return result;

        }

        public static HashSet<string> AllFormates()
        {
            HashSet<string> formates = new HashSet<string>();

            var allcultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            foreach (var item in allcultures)
            {
                var allformats = item.DateTimeFormat.GetAllDateTimePatterns();
                foreach (var f in allformats)
                {
                    formates.Add(f);
                }
            }

            return formates;
        }

        /// <summary>
        /// compare two date time down to minutes.
        /// </summary>
        /// <param name="timeone"></param>
        /// <param name="timetwo"></param>
        /// <returns></returns>
        public static bool EqualMinitues(DateTime timeone, DateTime timetwo)
        {
            return timeone.Year == timetwo.Year && timeone.DayOfYear == timetwo.DayOfYear && timeone.Hour == timetwo.Hour && timeone.Minute == timetwo.Minute;
        }

        /// <summary>
        /// Compare two value down to level of second.
        /// </summary>
        /// <param name="timeone"></param>
        /// <param name="timetwo"></param>
        /// <returns></returns>
        public static bool EqualSeconds(DateTime timeone, DateTime timetwo)
        {
            return timeone.Year == timetwo.Year && timeone.DayOfYear == timetwo.DayOfYear && timeone.Hour == timetwo.Hour && timeone.Minute == timetwo.Minute && timeone.Second == timetwo.Second;
        }

        private static DateTime UTCStartdate { get; set; } = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public static int GetWeekOfYear(DateTime datetime)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(datetime, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        public static string GetWeekName(DateTime datetime)
        {
            var weekName = GetWeekOfYear(datetime);
            string strWeekName = weekName > 9 ? weekName.ToString() : "0" + weekName.ToString();
            return datetime.Year.ToString() + "-" + strWeekName;
        }

        /// <summary>
        /// format of 2023-12.  year 2023, 12 calendar week. 
        /// </summary>
        /// <param name="WeekName"></param>
        /// <returns></returns>
        public static (int, int) ParseYearWeek(string WeekName)
        {
            int Year = 0;
            int Week = 0;
            int index = WeekName.IndexOf("-");
            if (index == -1)
            {
                index = WeekName.IndexOf("_");
            }

            if (index == -1)
            {
                return (0, 0);
            }
            string strYear = WeekName.Substring(0, index);
            string strWeek = WeekName.Substring(index + 1);
            if (strWeek.StartsWith("0"))
            {
                strWeek = strWeek.Substring(1);
            }

            if (int.TryParse(strYear, out int year))
            {
                Year = year;
            }
            if (int.TryParse(strWeek, out int week))
            {
                Week = week;
            }
            return (Year, Week);
        }

        /// <summary>
        /// format of 2023-11, year 2023, 11 month = nov.
        /// </summary>
        /// <param name="YearMonth"></param>
        /// <returns></returns>
        public static (int, int) ParseYearMonth(string YearMonth)
        {
            return ParseYearWeek(YearMonth);
        }

        public static long ToInt64(this DateTime date)
        {
            return date.Ticks;
        }

        public static DateTime FromInt64(this DateTime date, long dateTick)
        {
            return new DateTime(dateTick);
        }

        /// <summary>
        ///  Only valid to year of 2300. 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int DateToInt32(DateTime date)
        {
            TimeSpan last = date - UTCStartdate;
            return (int)last.TotalSeconds;
        }

        public static DateTime Int32ToDate(int value)
        {
            return UTCStartdate.AddSeconds(value);
        }

        private static string ToNumberString(int number)
        {
            if (number < 10)
            {
                return "0" + number.ToString();
            }
            else
            {
                return number.ToString();
            }
        }

        public static int HourToInt32(DateTime date)
        {
            string strDate = date.Year.ToString() + ToNumberString(date.Month) + ToNumberString(date.Day) + ToNumberString(date.Hour);
            return int.Parse(strDate);
        }

        public static DateTime Int32ToHour(int value)
        {
            string strValue = value.ToString();

            if (strValue.Length < 10)
            {
                return default;
            }

            int Year = int.Parse(strValue.Substring(0, 4));
            int Month = int.Parse(strValue.Substring(4, 2));
            int Day = int.Parse(strValue.Substring(6, 2));
            int Hour = int.Parse(strValue.Substring(8, 2));

            return new DateTime(Year, Month, Day, Hour, 0, 0);

        }


        public static int DayToInt32(DateTime date)
        {
            string strDate = date.Year.ToString() + ToNumberString(date.Month) + ToNumberString(date.Day);

            return int.Parse(strDate);
        }

        public static DateTime Int32ToDay(int value)
        {
            string strValue = value.ToString();
            if (strValue.Length < 8)
            {
                return default;
            }
            int Year = int.Parse(strValue.Substring(0, 4));
            int Month = int.Parse(strValue.Substring(4, 2));
            int Day = int.Parse(strValue.Substring(6, 2));
            return new DateTime(Year, Month, Day, 0, 0, 0);
        }



        public static int WeekToInt32(DateTime date)
        {
            var WeekInt = GetWeekOfYear(date);

            string strDate = date.Year.ToString() + ToNumberString(WeekInt);

            return int.Parse(strDate);
        }


        public static int MonthToInt32(DateTime date)
        {
            string strDate = date.Year.ToString() + ToNumberString(date.Month);

            return int.Parse(strDate);
        }
        public static DateTime Int32ToMonth(int value)
        {
            string strValue = value.ToString();
            if (strValue.Length < 6)
            {
                return default;
            }
            int Year = int.Parse(strValue.Substring(0, 4));
            int Month = int.Parse(strValue.Substring(4, 2));
            return new DateTime(Year, Month, 1, 0, 0, 0);
        }
    }
}

