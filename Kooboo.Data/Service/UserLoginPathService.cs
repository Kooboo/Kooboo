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
        }

        public static Database GlobalDatabase { get; set; }

   
        public static Table LastPath { get; set; }
          
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

            if (lower.StartsWith("/_admin") && !lower.StartsWith("/_admin/account") && !lower.StartsWith("/_admin/scripts") && !lower.StartsWith("/_admin/styles") && !lower.StartsWith("/_admin/help"))
            {  
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
