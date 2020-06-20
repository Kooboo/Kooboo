using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.DashBoard
{
  public static  class DashBoardHelper
    {

        public static List<VisitorLog> GetLogs(SiteDb sitedb, string WeekName = null)
        {
            if (string.IsNullOrEmpty(WeekName))
            {
                return sitedb.VisitorLog.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead);
            }
            else
            {
                var repo = sitedb.LogByWeek<VisitorLog>(WeekName);
                var list = repo.Take(false, 0, Kooboo.Data.AppSettings.MaxVisitorLogRead);
                repo.Close();
                return list;
            }
        }


    }
}
