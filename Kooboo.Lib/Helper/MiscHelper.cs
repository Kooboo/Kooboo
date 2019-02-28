//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
    public static class MiscHelper
    {
        public static string GetWeekName(DateTime datetime)
        {
            var weekname = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(datetime, System.Globalization.CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            string strweekname = weekname > 9 ? weekname.ToString() : "0" + weekname.ToString();

            return datetime.Year.ToString() + "-" + strweekname;
        }

        public static List<string> GetFolderWeekFileNames(string folder)
        { 
            List<string> names = new List<string>();
            string[] allfiles = null;

            if (System.IO.Directory.Exists(folder))
            {
                allfiles = System.IO.Directory.GetFiles(folder);
            }
             
            if (allfiles != null)
            {
                foreach (var item in allfiles)
                {
                    int lastslash =Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetLastSlash(item);
                    string name = item.Substring(lastslash + 1);

                    int index = name.LastIndexOf(".");
                    if (index > 0)
                    {
                        string weekname = name.Substring(0, index);
                        names.Add(weekname);
                    }
                }
            }
            return names; 
        }

    }
}
