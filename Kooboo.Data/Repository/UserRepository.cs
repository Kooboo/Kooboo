//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using Kooboo.Lib.Helper;
using Kooboo.Data.Helper;

namespace Kooboo.Data.Repository
{
    public class UserRepository
    {
        private string ValidateUrl = AccountUrlHelper.User("validate");
        private string ProfileUrl = AccountUrlHelper.User("profile");
        private string GetUserUrl = AccountUrlHelper.User("get");

        private string GetUserByEmailUrl = AccountUrlHelper.User("GetByEmail");

        private string ChangePasswordUrl = AccountUrlHelper.User("changepassword");
        private string ChangePasswordUrl2 = AccountUrlHelper.User("changepassword2");
        private string AddOrUpdateUserUrl = AccountUrlHelper.User("post");
        private string IsAdminUrl = AccountUrlHelper.User("isadmin");
        private string ChangeOrgUrl = AccountUrlHelper.User("ChangeOrg");
        private string OrganizationsUrl = AccountUrlHelper.User("Organizations");
        private string GetByTokenUrl = AccountUrlHelper.User("GetByToken");
        private string RegisterUrl = AccountUrlHelper.User("Register");

        private string ForgotPasswordTokenUrl = AccountUrlHelper.User("ForgotPasswordToken");
        public string ResetPasswordUrl = AccountUrlHelper.User("ResetPassword");

        public string GetOnlineServerUrlUrl = AccountUrlHelper.User("GetOnlineServerUrl");


        private Dictionary<Guid, User> Cache = new Dictionary<Guid, User>();
        private Dictionary<string, Guid> AccessToken = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);


        public void AddOrUpdateTemp(User user, bool Overwrite = false)
        {
            user.IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
            AddOrUpdateCache(user, Overwrite);
            AddOrUpdateLocal(user, Overwrite);
        }


        public bool RemoveLocal(Guid UserId)
        {
            if (Cache.ContainsKey(UserId))
            {
                Cache.Remove(UserId);
            }

            GlobalDb.LocalUser.Delete(UserId);

            return true;

        }

        public void AddOrUpdateCache(User user, bool overwrite = false)
        {
            lock (_locker)
            {
                if (overwrite)
                {
                    Cache[user.Id] = user;
                    return;
                }
                if (!Cache.ContainsKey(user.Id))
                {
                    Cache[user.Id] = user;
                }
            }
        }

        public void AddOrUpdateLocal(User user, bool overwrite = false)
        {
            lock (_locker)
            {
                if (overwrite)
                {
                    GlobalDb.LocalUser.AddOrUpdate(user);
                    return;
                }
                var olduser = GlobalDb.LocalUser.Get(user.Id);
                if (olduser == null)
                {
                    GlobalDb.LocalUser.AddOrUpdate(user);
                }
            }
        }

        public User Get(string nameOrGuid)
        {
            Guid userid;
            if (!Guid.TryParse(nameOrGuid, out userid))
            {
                userid = IDGenerator.GetId(nameOrGuid);
            }
            return this.Get(userid);
        }

        public User Get(Guid id)
        {
            if (!Kooboo.Data.Service.UserLoginService.IsAllow(id))
            {
                return null;
            }

            if (this.Cache.ContainsKey(id))
            {
                return this.Cache[id];
            }

            var user = GlobalDb.LocalUser.Get(id);

            if (user == null)
            {
                user = Kooboo.Data.Service.UserLoginService.GetDefaultUser(id.ToString());
                if (user != null)
                {
                    this.Cache[user.Id] = user;
                }
            }

            if (user == null)
            {
                // string hashcode = RSAHelper.Encrypt(id.ToString());
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("id", id.ToString());
                user = HttpHelper.Post<User>(this.GetUserUrl, para);

                if (user != null)
                {
                    AddOrUpdateTemp(user);
                }
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("email", email);
            return HttpHelper.Get<User>(this.GetUserByEmailUrl, para);
        }

        public string GetServerUrl(string email)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("email", email);
            string url = HttpHelper.Get<string>(this.GetOnlineServerUrlUrl, para);

            if (!string.IsNullOrWhiteSpace(url) && !url.ToLower().StartsWith("http"))
            {
                url = "https://" + url;
            }
            return url;
        }

        public User Profile(Guid id)
        {
            User user = null;
            if (this.Cache.ContainsKey(id))
            {
                user = this.Cache[id];
                if (user != null && user.PasswordHash != default(Guid) && user.CurrentOrgId != default(Guid))
                {
                    return user;
                }
            }
            user = GlobalDb.LocalUser.Get(id);

            if (user != null && (user.PasswordHash != default(Guid) || !string.IsNullOrWhiteSpace(user.Password)) && user.CurrentOrgId != default(Guid))
            {
                return user;
            }

            if (user == null)
            {
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("id", id.ToString());
                user = Lib.Helper.HttpHelper.Post<User>(this.ProfileUrl, para);

                if (user != null)
                {
                    AddOrUpdateTemp(user);
                }
                return user;
            }
            return user;
        }

        public bool IsDefaultUser(User user)
        {
            if (user == null || Data.AppSettings.DefaultUser == null || string.IsNullOrWhiteSpace(AppSettings.DefaultUser.UserName))
            {
                return false;
            }
            if (user.UserName.ToUpper() == Data.AppSettings.DefaultUser.UserName.ToUpper())
            {
                return true;
            }
            return false;
        }

        public bool AddOrUpdate(User user)
        {
            User updateuser = Kooboo.Lib.Serializer.Copy.DeepCopy<User>(user);

            string userJson = Lib.Helper.JsonHelper.Serialize(updateuser);

            bool isSuccess = false;
            if (IsDefaultUser(user))
            {
                isSuccess = true;
            }
            else
            {
                isSuccess = HttpHelper.Post<bool>(AddOrUpdateUserUrl, userJson);
            }

            if (isSuccess)
            {
                AddOrUpdateTemp(user, true);
                return true;
            }
            return false;
        }

        // register and return url. 
        public bool Register(User User)
        {
            User updateuser = Kooboo.Lib.Serializer.Copy.DeepCopy<User>(User);
            string userJson = Lib.Helper.JsonHelper.Serialize(updateuser);
            return HttpHelper.Post<bool>(RegisterUrl, userJson);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            //clean local cache and local user.           
            Dictionary<string, string> query = new Dictionary<string, string>();

            query.Add("UserName", username);
            query.Add("oldPassword", oldPassword);
            query.Add("newPassword", newPassword);

            var user = HttpHelper.Post<User>(this.ChangePasswordUrl2, query);

            if (user != null)
            {
                user.Password = newPassword;

                if (this.Cache.ContainsKey(user.Id))
                {
                    this.Cache[user.Id] = user;
                }
                GlobalDb.LocalUser.AddOrUpdate(user);
                return true;
            }

            return false;
        }

        public User Validate(string username, string password)
        {
            if (username == null || password == null)
            {
                return null;
            }

            if (!Kooboo.Data.Service.UserLoginService.IsAllow(username))
            {
                return null;
            }

            Dictionary<String, string> para = new Dictionary<string, string>();
            para.Add("UserName", username);
            para.Add("Password", password);

            // check cache first for performance. 
            Guid userid = IDGenerator.GetId(username);

            if (this.Cache.ContainsKey(userid) && !Data.AppSettings.IsOnlineServer)
            {
                var cacheuser = this.Cache[userid];
                if (cacheuser.Password != null && Kooboo.Data.Service.UserLoginService.IsValidPassword(cacheuser, password))
                {
                    return cacheuser;
                }
            }

            var user = Lib.Helper.HttpHelper.Post<User>(ValidateUrl, para);

            if (user == null)
            {
                if (this.Cache.ContainsKey(userid))
                {
                    user = this.Cache[userid];
                }
                else
                {
                    user = GlobalDb.LocalUser.Get(username);
                }

                if (user == null || string.IsNullOrWhiteSpace(user.Password))
                {
                    user = Kooboo.Data.Service.UserLoginService.GetDefaultUser(username);
                }

                if (user != null)
                {
                    if (user.Password != null && Kooboo.Data.Service.UserLoginService.IsValidPassword(user, password))
                    {
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                user.Password = password;
                AddOrUpdateTemp(user, true);
            }
            return user;
        }

        public string GetUserName(Guid userId)
        {
            if (userId == default(Guid))
            {
                return null;
            }
            var user = Get(userId);
            if (user != null)
            {
                return user.UserName;
            }
            return string.Empty;
        }

        public bool IsAdmin(Guid organizationId, Guid UserId)
        {
            var org = GlobalDb.Organization.Get(organizationId);
            if (org != null && org.AdminUser == UserId)
            {
                return true;
            }

            return false;
            //Dictionary<string, string> para = new Dictionary<string, string>();
            //para.Add("userid", RSAHelper.Encrypt(UserId.ToString()));
            //para.Add("organizationId", RSAHelper.Encrypt(organizationId.ToString()));
            //var paramStr = Lib.Helper.JsonHelper.Serialize(para);
            //return HttpHelper.Post<bool>(IsAdminUrl, paramStr);
        }

        public User ChangeOrg(Guid UserId, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("UserId", UserId.ToString());
            para.Add("organizationId", organizationId.ToString());

            var user = HttpHelper.Post<User>(ChangeOrgUrl, para);
            if (user != null)
            {
                if (this.Cache.ContainsKey(user.Id))
                {
                    var olduser = this.Cache[user.Id];
                    olduser.CurrentOrgId = user.CurrentOrgId;
                    olduser.CurrentOrgName = user.CurrentOrgName;
                    olduser.IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
                }

                var local = GlobalDb.LocalUser.Store.get(user.Id);
                if (local != null)
                {
                    local.CurrentOrgId = user.CurrentOrgId;
                    local.CurrentOrgName = user.CurrentOrgName;
                    local.IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
                    GlobalDb.LocalUser.AddOrUpdate(local);
                }
            }

            return user;
        }

        public List<Organization> Organizations(Guid userId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("userId", userId.ToString());
            return HttpHelper.Post<List<Organization>>(OrganizationsUrl, para);
        }

        private User _defaultUser;
        private object _locker = new object();
        public User DefaultUser
        {
            get
            {
                if (_defaultUser == null)
                {
                    lock (_locker)
                    {
                        if (_defaultUser == null)
                        {
                            _defaultUser = new User()
                            {
                                UserName = DataConstants.DefaultUserName,
                                CurrentOrgId = Lib.Security.Hash.ComputeGuidIgnoreCase(DataConstants.DefaultUserName),
                                CurrentOrgName = DataConstants.DefaultUserName
                            };
                        }
                    }
                }
                return _defaultUser;
            }
        }

        // this is to get orgname for folder. 
        internal User GetLocalUserByOrgId(Guid OrgId)
        {
            foreach (var item in this.Cache)
            {
                if (item.Value.CurrentOrgId == OrgId)
                {
                    return item.Value;
                }
            }

            foreach (var item in GlobalDb.LocalUser.All())
            {
                if (item.CurrentOrgId == OrgId)
                {
                    return item;
                }
            }
            return null;
        }

        public User GetByToken(string token)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("mytoken", token);
            var user = HttpHelper.Post<User>(GetByTokenUrl, para);
            if (user != null)
            {
                if (user.PasswordHash != default(Guid))
                {
                    AddOrUpdateTemp(user, true);
                }
                else
                {
                    AddOrUpdateTemp(user, false);
                }
            }
            return user;
        }

        public Guid ForgotPasswordToken(string email)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("email", email);
            return HttpHelper.Get<Guid>(this.ForgotPasswordTokenUrl, para);
        }

        public bool ResetPassword(Guid Token, string newpasspord)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Token", Token.ToString());
            para.Add("newpasspord", newpasspord);
            return HttpHelper.Post<bool>(this.ResetPasswordUrl, para);
        }
    }
}
