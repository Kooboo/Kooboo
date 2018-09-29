//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Upgrade
{
   public static  class UpgradeManager
    { 
        private static List<IApplicationUpgrade> AppUpgraders()
        {
            var list = new List<IApplicationUpgrade>();
           /// list.Add(new ContentDataSourceUpgrade());
            return list; 
        }

        private static List<IWebSiteUpgrade> SiteUpgraders()
        {
            var list = new List<IWebSiteUpgrade>();
            list.Add(new SiteContentDataSourceUpgrade());
            return list;
        }

        public static List<IApplicationUpgrade> GetAppUpgraderList(System.Version version)
        {
            if (version == Data.AppSettings.Version)
            {
                return new List<IApplicationUpgrade>();
            }

            var result = new List<IApplicationUpgrade>();

            foreach (var item in AppUpgraders())
            {
                if (item.LowerVersion >= version && item.UpVersion <= Data.AppSettings.Version)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<IWebSiteUpgrade> GetSiteUpgraderList(System.Version version)
        {
            if (version == Data.AppSettings.Version)
            {
                return new List<IWebSiteUpgrade>();
            }

            var result = new List<IWebSiteUpgrade>();

            foreach (var item in SiteUpgraders())
            {
                if (item.LowerVersion >= version && item.UpVersion <= Data.AppSettings.Version)
                {
                    result.Add(item);
                }
            }
            return result.OrderBy(o=>o.LowerVersion).ToList();
        }

        public static System.Version GetVersion()
        {
            var x = Kooboo.Data.GlobalDb.GlobalSetting.Store.FullScan(o => o.Name == "Version").FirstOrDefault();
            if (x != null && Lib.Helper.DataTypeHelper.IsInt(x.Values))
            {
                return ParseVersion(x.Values);  
            }
            return new Version("0.0.0.0"); 
        }

        public static void SetVersion(System.Version version)
        { 
            Data.GlobalDb.GlobalSetting.AddOrUpdate(new Data.Models.GlobalSetting() { Name = "Version", Values = version.ToString() });  
        }


        public static System.Version ParseVersion(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                input = "0.0.0.1"; 
            }

           if (input.IndexOf(".") ==-1)
            {
                input = "0." + input + ".0.0"; 
            }

            System.Version version = null; 

            if (System.Version.TryParse(input, out version))
            {
                return version; 
            }

            return new Version("0.0.0.1"); 
           
        }
       
    }
     
}
