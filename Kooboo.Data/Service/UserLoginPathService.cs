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

                if (lower.EndsWith(".html") || lower.EndsWith(".js") || lower.EndsWith(".css") || lower.EndsWith(".png"))
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
                    return ClarifyPath(obj.ToString());
                }
            }
            return null;
        }

        public static string ClarifyPath(string path)
        {
            if (path == null)
            {
                return null;
            }
            path = path.ToLower();

            if (path.StartsWith("/_admin/emails"))
            {
                return "/_Admin/Emails/Inbox";
            }
            else if (path.StartsWith("/_admin/market"))
            {
                return "/_Admin/Market/Index";
            }
            else if (path.StartsWith("/_admin/domains"))
            {
                return "/_Admin/Domains";
            }
            else
            {
                ///_Admin/Site?SiteId=ef9b3c71-f09b-a23c-431c-3d8d40905226
                if (path.Contains("siteid="))
                {
                    var siteid = GetSiteIdFromPath(path);
                    if (siteid != null)
                    {
                        return "/_Admin/Site?SiteId=" + siteid;
                    }
                }
            }

            return "/_Admin/Sites";
        }

        public static string GetSiteIdFromPath(string path)
        {

            ///_Admin/Site?SiteId=ef9b3c71-f09b-a23c-431c-3d8d40905226
            string siteid = null;

            int index = path.IndexOf("siteid=");
            if (index > -1)
            {
                var andindex = path.IndexOf("&", index);
                if (andindex > -1)
                {
                    siteid = path.Substring(index + 7, andindex - index - 7);
                }
                else

                {
                    siteid = path.Substring(index + 7);
                }
            }
             
            if (siteid !=null)
            {
                Guid key = default(Guid);
                if (System.Guid.TryParse(siteid, out key))
                {
                    return siteid; 
                }
            } 
            return null;
        }

    }


}
