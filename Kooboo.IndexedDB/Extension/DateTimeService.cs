//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
   public static class DateTimeService
    {
       public static DateTime ConvertIntToDateTime(int dayint, int totalSeconds)
       {
         return  GlobalSettings.UTCStartdate.AddDays(dayint).AddSeconds(totalSeconds); 
       }
    }
}
