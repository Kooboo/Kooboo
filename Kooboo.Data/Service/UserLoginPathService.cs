//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Dynamic;
using System;
using System.Collections.Generic; 

namespace Kooboo.Data.Service
{  
    public class UserLoginPathService
    {
        static UserLoginPathService()
        {
            GlobalDatabase = Kooboo.Data.DB.Global();
    
            var setting = new Setting();
            setting.AppendColumn("Path", typeof(string), 800);

            LastPath = GlobalDatabase.GetOrCreateTable("userpath", setting);

            IgnorePath = new List<string>();
            IgnorePath.Add("/_admin/account");
            IgnorePath.Add("/_admin/scripts");
            IgnorePath.Add("/_admin/styles");
            IgnorePath.Add("/_admin/images");
            IgnorePath.Add("/_admin/help");
            IgnorePath.Add("_ddmin/sites/edit");

        }

        public static Database GlobalDatabase { get; set; }

   
        public static Table LastPath { get; set; }

        private static List<string> IgnorePath { get; set; }
          
        public static void UpdateLastPath(BackendLog log)
        {
            if (log == null || log.UserId == default(Guid) || log.Url == null)
            {
                return;
            }

            if (log.Url.Length > 700)
            {
                return;  // too long. 
            }

            var lower = log.Url.ToLower(); 

            if (lower.StartsWith("/_admin"))
            {
                foreach (var item in IgnorePath)
                {
                    if (lower.StartsWith(item))
                    {
                        return; 
                    }
                }

               if (lower.EndsWith(".html") || lower.EndsWith(".js")|| lower.EndsWith(".css") || lower.EndsWith(".png"))
                {
                    return; 
                }

                var find = LastPath.Get(log.UserId);
                if (find != null && find.ContainsKey("_id"))
                {
                    LastPath.UpdateColumn(find["_id"], "Path", log.Url);
                }
                else
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data["_id"] = log.UserId;
                    data["Path"] = log.Url;
                    LastPath.Add(data);
                }

                LastPath.Close();

            } 
              
        }

        public static string GetLastPath(Guid UserId)
        {

            var find = LastPath.Get(UserId);
            if (find != null && find.ContainsKey("Path"))
            {
                var obj = find["Path"];
                if (obj != null)
                {
                    return obj.ToString();
                }
            }
            return null;
        }

    }


}
