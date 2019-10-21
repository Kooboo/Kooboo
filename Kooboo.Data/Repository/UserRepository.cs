//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Lib.Helper;
using Kooboo.Data.Helper;

namespace Kooboo.Data.Repository
{
    public class UserRepository
    {
        private string _validateUrl = AccountUrlHelper.User("validate");
        private string _profileUrl = AccountUrlHelper.User("profile");
        private string _getUserUrl = AccountUrlHelper.User("get");

        private string _getUserByEmailUrl = AccountUrlHelper.User("GetByEmail");

        private string _changePasswordUrl = AccountUrlHelper.User("changepassword");
        private string _changePasswordUrl2 = AccountUrlHelper.User("changepassword2");
        private string _addOrUpdateUserUrl = AccountUrlHelper.User("post");
        private string _isAdminUrl = AccountUrlHelper.User("isadmin");
        private string _changeOrgUrl = AccountUrlHelper.User("ChangeOrg");
        private string _organizationsUrl = AccountUrlHelper.User("Organizations");
        private string _getByTokenUrl = AccountUrlHelper.User("GetByToken");
        private string _registerUrl = AccountUrlHelper.User("Register");

        private string _forgotPasswordTokenUrl = AccountUrlHelper.User("ForgotPasswordToken");
        public string ResetPasswordUrl = AccountUrlHelper.User("ResetPassword");

        public string GetOnlineServerUrlUrl = AccountUrlHelper.User("GetOnlineServerUrl");


        private Dictionary<Guid, User> _cache = new Dictionary<Guid, User>();
        private Dictionary<string, Guid> _accessToken = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);


        public void AddOrUpdateTemp(User user, bool overwrite = false)
        {
            user.IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
            AddOrUpdateCache(user, overwrite);
            AddOrUpdateLocal(user, overwrite);
        }


        public bool RemoveLocal(Guid userId)
        {
            if (_cache.ContainsKey(userId))
            {
                _cache.Remove(userId); 
            }

            GlobalDb.LocalUser.Delete(userId);

            return true; 

        }

        public void AddOrUpdateCache(User user, bool overwrite = false)
        {
            lock (_locker)
            {
                if (overwrite)
                {
                    _cache[user.Id] = user;
                    return;
                }
                if (!_cache.ContainsKey(user.Id))
                {
                    _cache[user.Id] = user;
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
            if (!Guid.TryParse(nameOrGuid, out var userid))
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

            if (this._cache.ContainsKey(id))
            {
                return this._cache[id];
            }

            var user = GlobalDb.LocalUser.Get(id);

            if (user == null)
            {
                user = Kooboo.Data.Service.UserLoginService.GetDefaultUser(id.ToString());
                if (user != null)
                {
                    this._cache[user.Id] = user;
                }
            }

            if (user == null)
            {
                // string hashcode = RSAHelper.Encrypt(id.ToString());
                Dictionary<string, string> para = new Dictionary<string, string> {{"id", id.ToString()}};
                user = HttpHelper.Post<User>(this._getUserUrl, para);

                if (user != null)
                {
                    AddOrUpdateTemp(user);
                }
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            Dictionary<string, string> para = new Dictionary<string, string> {{"email", email}};
            return HttpHelper.Get<User>(this._getUserByEmailUrl, para);
        }

        public string GetServerUrl(string email)
        {
            Dictionary<string, string> para = new Dictionary<string, string> {{"email", email}};
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
            if (this._cache.ContainsKey(id))
            {
                user = this._cache[id];
                if (user != null && user.PasswordHash != default(Guid) && user.CurrentOrgId != default)
                {
                    return user;
                }
            }
            user = GlobalDb.LocalUser.Get(id);

            if (user != null && (user.PasswordHash != default || !string.IsNullOrWhiteSpace(user.Password)) && user.CurrentOrgId != default)
            {
                return user;
            }

            if (user == null)
            {
                Dictionary<string, string> para = new Dictionary<string, string> {{"id", id.ToString()}};
                user = Lib.Helper.HttpHelper.Post<User>(this._profileUrl, para);

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
            return user.UserName.ToUpper() == Data.AppSettings.DefaultUser.UserName.ToUpper();
        }

        public bool AddOrUpdate(User user)
        {
            User updateuser = Kooboo.Lib.Serializer.Copy.DeepCopy<User>(user);

            string userJson = Lib.Helper.JsonHelper.Serialize(updateuser);

            bool isSuccess = false;
            isSuccess = IsDefaultUser(user) || HttpHelper.Post<bool>(_addOrUpdateUserUrl, userJson);

            if (isSuccess)
            {
                AddOrUpdateTemp(user, true);
                return true;
            }
            return false;
        }

        // register and return url. 
        public bool Register(User user)
        {
            User updateuser = Kooboo.Lib.Serializer.Copy.DeepCopy<User>(user);
            string userJson = Lib.Helper.JsonHelper.Serialize(updateuser);
            return HttpHelper.Post<bool>(_registerUrl, userJson);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            //clean local cache and local user.           
            Dictionary<string, string> query = new Dictionary<string, string>
            {
                {"UserName", username}, {"oldPassword", oldPassword}, {"newPassword", newPassword}
            };


            var user = HttpHelper.Post<User>(this._changePasswordUrl2, query);

            if (user != null)
            {
                user.Password = newPassword;

                if (this._cache.ContainsKey(user.Id))
                {
                    this._cache[user.Id] = user;
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

            Dictionary<String, string> para = new Dictionary<string, string>
            {
                {"UserName", username}, {"Password", password}
            };

            // check cache first for performance. 
            Guid userid = IDGenerator.GetId(username);

            if (this._cache.ContainsKey(userid) && !Data.AppSettings.IsOnlineServer)
            {
                var cacheuser = this._cache[userid];
                if (cacheuser.Password != null && cacheuser.Password == password)
                {
                    return cacheuser;
                }
            }

            var user = Lib.Helper.HttpHelper.Post<User>(_validateUrl, para);

            if (user == null)
            {
                user = this._cache.ContainsKey(userid) ? this._cache[userid] : GlobalDb.LocalUser.Get(username);

                if (user == null || string.IsNullOrWhiteSpace(user.Password))
                {
                    user = Kooboo.Data.Service.UserLoginService.GetDefaultUser(username);
                }

                if (user != null)
                {
                    if (user.Password != null && user.Password == password)
                    {
                        return user;
                    }

                    return null;
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

        public bool IsAdmin(Guid organizationId, Guid userId)
        {
            var org = GlobalDb.Organization.Get(organizationId);
            if (org != null && org.AdminUser == userId)
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

        public User ChangeOrg(Guid userId, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                {"UserId", userId.ToString()}, {"organizationId", organizationId.ToString()}
            };

            var user = HttpHelper.Post<User>(_changeOrgUrl, para);
            if (user != null)
            {
                if (this._cache.ContainsKey(user.Id))
                {
                    var olduser = this._cache[user.Id];
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
            Dictionary<string, string> para = new Dictionary<string, string> {{"userId", userId.ToString()}};
            return HttpHelper.Post<List<Organization>>(_organizationsUrl, para);
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
                            _defaultUser = new User
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
        internal User GetLocalUserByOrgId(Guid orgId)
        {
            foreach (var item in this._cache)
            {
                if (item.Value.CurrentOrgId == orgId)
                {
                    return item.Value;
                }
            }

            return GlobalDb.LocalUser.All().FirstOrDefault(item => item.CurrentOrgId == orgId);
        }

        public User GetByToken(string token)
        {
            Dictionary<string, string> para = new Dictionary<string, string> {{"mytoken", token}};
            var user = HttpHelper.Post<User>(_getByTokenUrl, para);
            if (user != null)
            {
                AddOrUpdateTemp(user, user.PasswordHash != default);
            }
            return user;
        }

        public Guid ForgotPasswordToken(string email)
        {
            Dictionary<string, string> para = new Dictionary<string, string> {{"email", email}};
            return HttpHelper.Get<Guid>(this._forgotPasswordTokenUrl, para);
        }

        public bool ResetPassword(Guid token, string newpasspord)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                {"Token", token.ToString()}, {"newpasspord", newpasspord}
            };
            return HttpHelper.Post<bool>(this.ResetPasswordUrl, para);
        } 
    }
}
