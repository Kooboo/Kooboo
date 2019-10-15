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

            LastPath = GlobalDatabase.GetOrCreateTable("userpath", setting);

            IgnorePath = new List<string>
            {
                "/_admin/account",
                "/_admin/scripts",
                "/_admin/styles",
                "/_admin/images",
                "/_admin/help",
                "_ddmin/sites/edit"
            };
        }

        public static Database GlobalDatabase { get; set; }

        public static Table LastPath { get; set; }

        private static List<string> IgnorePath { get; set; }

        public static void UpdateLastPath(BackendLog log)
        {
            if (log == null || log.UserId == default(Guid) || log.Url == null)
                return;

            if (log.Url.Length > 700)
                return; // too long.

            var lower = log.Url.ToLower();

            if (lower.StartsWith("/_admin"))
            {
                if (IgnorePath.Any(item => lower.StartsWith(item)))
                    return;

                if (lower.EndsWith(".html") || lower.EndsWith(".js") || lower.EndsWith(".css") || lower.EndsWith(".png"))
                    return;

                var find = LastPath.Get(log.UserId);
                if (find != null && find.ContainsKey("_id"))
                {
                    LastPath.UpdateColumn(find["_id"], "Path", log.Url);
                }
                else
                {
                    Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        ["_id"] = log.UserId, ["Path"] = log.Url
                    };
                    LastPath.Add(data);
                }

                LastPath.Close();
            }
        }

        public static string GetLastPath(Guid userId)
        {
            var find = LastPath.Get(userId);
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
                siteid = andindex > -1 ? path.Substring(index + 7, andindex - index - 7) : path.Substring(index + 7);
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

            user.Language = !string.IsNullOrWhiteSpace(AppSettings.CmsLang) ? AppSettings.CmsLang : "en";

            user.PasswordHash = System.Guid.NewGuid();  // Fake.

            return user;
        }

        public static User GetDefaultUser(string nameOrId)
        {
            if (nameOrId == null)
            {
                return null;
            }

            if (Data.AppSettings.DefaultUser != null)
            {
                Guid userid = default(Guid);
                if (!System.Guid.TryParse(nameOrId, out userid))
                {
                    userid = Lib.Security.Hash.ComputeGuidIgnoreCase(nameOrId);
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

            Guid userId = Lib.Security.Hash.ComputeGuidIgnoreCase(username);

            return IsAllow(userId);
        }

        public static bool IsAllow(Guid userId)
        {
            if (Data.AppSettings.AllowUsers == null || !Data.AppSettings.AllowUsers.Any())
            {
                return true;
            }

            if (Data.AppSettings.AllowUsers.Contains(userId))
            {
                return true;
            }

            if (Data.AppSettings.DefaultUser != null)
            {
                var defaultid = Lib.Security.Hash.ComputeGuidIgnoreCase(Data.AppSettings.DefaultUser.UserName);
                if (defaultid == userId)
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

            return user.PasswordHash != default(Guid) ? user.PasswordHash.ToString() : null;
        }
    }
}