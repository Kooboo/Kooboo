//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Helper
{
    public static class MiscHelper
    {
        public static string GetWeekName(DateTime dateTime)
        {
            var WeekName = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            string strWeekName = WeekName > 9 ? WeekName.ToString() : "0" + WeekName.ToString();


            int year = dateTime.Year;

            if (WeekName > 51)
            {
                year = dateTime.AddDays(-21).Year;
            }

            return year.ToString() + "-" + strWeekName;
        }


        public static List<string> GetFolderWeekFileNames(string folder)
        {
            List<string> names = new List<string>();
            string[] allFiles = null;

            if (System.IO.Directory.Exists(folder))
            {
                allFiles = System.IO.Directory.GetFiles(folder);
            }

            if (allFiles != null)
            {
                foreach (var item in allFiles)
                {
                    int LastSlash = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetLastSlash(item);
                    string name = item.Substring(LastSlash + 1);

                    int index = name.LastIndexOf(".");
                    if (index > 0)
                    {
                        string WeekName = name.Substring(0, index);
                        names.Add(WeekName);
                    }
                }
            }
            return names;
        }

        public static int GetPageCount(int totalcount, int pagesize)
        {
            if (totalcount <= 0)
            {
                return 0;
            }

            if (pagesize <= 1)
            {
                pagesize = 1;
            }

            int number = (int)totalcount / pagesize;

            int newtotal = pagesize * number;

            return newtotal < totalcount ? number + 1 : number;
        }
    }
}
