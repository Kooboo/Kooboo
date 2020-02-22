//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Data.Service
{
    public class UserLoginService
    {
        static UserLoginService()
        {
            GlobalDatabase = Kooboo.Data.DB.Global();

            var setting = new Setting();
            setting.AppendColumn("Path", typeof(string), 800);

            LastPath = Data.DB.GetOrCreateTable(GlobalDatabase, "userpath", setting);

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
            else if (path.StartsWith("/_admin/site"))
            {
                return "/_Admin/Sites";
            }
            //else
            //{
            //    ///_Admin/Site?SiteId=ef9b3c71-f09b-a23c-431c-3d8d40905226
            //    if (path.Contains("siteid="))
            //    {
            //        var siteid = GetSiteIdFromPath(path);
            //        if (siteid != null)
            //        {
            //            return "/_Admin/Site?SiteId=" + siteid;
            //        }
            //    }
            //}

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

            if (siteid != null)
            {
                Guid key = default(Guid);
                if (System.Guid.TryParse(siteid, out key))
                {
                    return siteid;
                }
            }
            return null;
        }

        public static User LoginDefaultUser(string username, string password)
        {
            if (username == null || password == null)
            {
                return null;
            }

            if (Data.AppSettings.DefaultUser != null)
            {
                if (username.ToLower() == AppSettings.DefaultUser.UserName && password == AppSettings.DefaultUser.Password)
                {
                    User user = _defaultUser();

                    return user;
                }
            }

            return null;
        }

        private static User _defaultUser()
        {
            var user = new User();
            if (Data.AppSettings.DefaultUser != null)
            {
                user.UserName = AppSettings.DefaultUser.UserName;
                user.Password = AppSettings.DefaultUser.Password;

            }
            user.CurrentOrgName = user.UserName;
            user.CurrentOrgId = Lib.Security.Hash.ComputeGuidIgnoreCase(user.UserName);

            user.Language = AppSettings.CmsLang;
             
            user.PasswordHash = System.Guid.NewGuid(); //Fake. 

            return user;
        }

        public static User GetDefaultUser(string NameOrId)
        {
            if (NameOrId == null)
            {
                return null;
            }

            if (Data.AppSettings.DefaultUser != null)
            {
                Guid userid = default(Guid);
                if (!System.Guid.TryParse(NameOrId, out userid))
                {
                    userid = Lib.Security.Hash.ComputeGuidIgnoreCase(NameOrId);
                }

                if (userid == Data.AppSettings.DefaultUser.Id)
                {
                    return _defaultUser();
                }
            }
            return null;
        }

        public static bool IsAllow(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            if (Data.AppSettings.AllowUsers == null)
            {
                return true;
            }

            if (Data.AppSettings.DefaultUser != null && Lib.Helper.StringHelper.IsSameValue(Data.AppSettings.DefaultUser.UserName, username))
            {
                return true;
            }

            Guid UserId = Lib.Security.Hash.ComputeGuidIgnoreCase(username);

            return IsAllow(UserId);
        }

        public static bool IsAllow(Guid UserId)
        {
            if (Data.AppSettings.AllowUsers == null || !Data.AppSettings.AllowUsers.Any())
            {
                return true;
            }

            if (Data.AppSettings.AllowUsers.Contains(UserId))
            {
                return true;
            }

            if (Data.AppSettings.DefaultUser != null)
            {
                var defaultid = Lib.Security.Hash.ComputeGuidIgnoreCase(Data.AppSettings.DefaultUser.UserName);
                if (defaultid == UserId)
                {
                    return true;
                }

            }

            return false;
        }
         
        public static string GetUserPassword(User user)
        {
            if (user == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                var hash = Lib.Helper.IDHelper.ParseKey(user.Password);
                if (hash != default(Guid))
                {
                    return user.Password;
                }
            }

            if (user.PasswordHash != default(Guid))
            {
                return user.PasswordHash.ToString();
            }
            return null;
        }
         
        public static bool IsValidPassword(User user, string password)
        {
            if (Guid.TryParse(password, out Guid PassHash))
            {
                if (user.PasswordHash == PassHash)
                {
                    return true;
                }
            }
            else
            {
                if (user.Password == password)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
