using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Service
{
    public static class UpGradeService
    {

        public static void UpgradeFix()
        {
            FixBefore13();
        }

        public static void FixBefore13()
        {
            //if (Kooboo.Data.AppSettings.Version.Major ==1 && Data.AppSettings.Version.Minor < 3)
            //{
            foreach (var item in Kooboo.Data.GlobalDb.WebSites.All())
            {
                try
                {
                    Kooboo.Sites.Service.WebSiteService.VerifyFrontendEvent(item);
                }
                catch (Exception ex)
                {
                    Kooboo.Data.Log.Instance.Exception.Write(ex.Message + ex.Source + ex.StackTrace); 
                }
   
            }
            //}
        }

    }
}
