//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    if (formate[i] =='y' || formate[i] =='Y')
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
            else if (formate[len -1] == 'y' || formate[len-1] == 'Y')
            {

                for (int i = len -1; i > 0; i--)
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

                    return formate.Substring(0, i+1);
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

        // compare till minutes. 
        public static bool EqualMinitues(DateTime timeone, DateTime timetwo)
        {
            return timeone.Year == timetwo.Year && timeone.DayOfYear == timetwo.DayOfYear && timeone.Hour == timetwo.Hour && timeone.Minute == timetwo.Minute; 
        }    
    }
}

