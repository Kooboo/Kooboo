//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Web.Api.V2;

namespace Kooboo.Web.Api.Implementation
{
    public class UserApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "User";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public virtual MetaResponse Login(string UserName, string Password, ApiCall apiCall)
        {
            if (!Kooboo.Data.Service.UserLoginProtection.CanTryLogin(UserName, apiCall.Context.Request.IP, Password))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("user or ip temporarily lockout", apiCall.Context));
            }

            var user = Kooboo.Data.GlobalDb.Users.Validate(UserName, Password);

            if (user == null)
            {
                Data.Service.UserLoginProtection.AddLoginFail(UserName, apiCall.Context.Request.IP);
            }
            else
            {
                Data.Service.UserLoginProtection.AddLoginOK(UserName, apiCall.Context.Request.IP);
            }

            if (user != null)
            {
                string returnUrl = apiCall.GetValue("returnurl");
                if (returnUrl != null)
                {
                    returnUrl = System.Web.HttpUtility.UrlDecode(returnUrl);
                    returnUrl = System.Web.HttpUtility.UrlDecode(returnUrl);
                    // the redirect from access token. 
                    if (returnUrl != null && returnUrl.ToLower().Contains("accesstoken"))
                    {
                        returnUrl = null;
                    }
                    else
                    {
                        var lower = returnUrl.ToLower();

                        if (lower == "/_admin" || lower == "/_admin/" || lower == "\\_admin" || lower == "\\_admin\\")
                        {
                            returnUrl = null;
                        }
                    }
                }

                var response = new MetaResponse();

                response.Success = true;

                string redirct = Kooboo.Web.Service.UserService.GetLoginRedirectUrl(apiCall.Context, user, apiCall.Context.Request.Url, returnUrl);

                if (apiCall.GetBoolValue("withToken"))
                {
                    string token = null;

                    if (AppSettings.DefaultUser != null && user.UserName == AppSettings.DefaultUser.UserName)
                    {
                        token = Service.UserService.GenerateTokenFromLocal(user);
                    }
                    else
                    {
                        token = user.OneTimeToken;
                    }

                    if (token == null && !Kooboo.Data.AppSettings.IsOnlineServer)
                    {
                        token = Service.UserService.GenerateTokenFromLocal(user);
                    }

                    apiCall.Context.Response.Headers.Add("access_token", token);
                    Kooboo.Data.Cache.AccessTokenCache.SetToken(user.Id, token);
                }

#if DEBUG

                redirct = Lib.Helper.UrlHelper.RelativePath(redirct);
#endif

                response.Model = redirct;
                // resposne redirect url. for online and local version...  
                return response;
            }
            var noresponse = new MetaResponse();
            noresponse.Success = false;
            noresponse.Messages.Add(Data.Language.Hardcoded.GetValue("User name or password not valid", apiCall.Context));
            return noresponse;
        }

        public virtual string GetRegisterRedirectUrl(User user, string currentrequesturl)
        {
            var newuser = Kooboo.Data.GlobalDb.Users.Validate(user.UserName, user.Password);  // this is to ensure that local has the user detail info like username, password, orgname, etc...  
            return Lib.Helper.UrlHelper.Combine(currentrequesturl, "/_admin");
        }

        public virtual MetaResponse Register(string UserName, string Password, string email, ApiCall apiCall)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(email))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Username or password or email not provided", apiCall.Context));
            }

            if (!Lib.Helper.StringHelper.IsValidUserName(UserName))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Invalid User name formate", apiCall.Context));
            }

            if (Lib.Helper.StringHelper.IsUserNamePrefixReserved(UserName))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Reserved user name prefix", apiCall.Context));
            }

            var user = new User();
            user.UserName = UserName;
            user.Password = Password;
            user.EmailAddress = email;
            string acceptlang = apiCall.Context.Request.Headers["Accept-Language"];
            user.Language = Kooboo.Data.Language.LanguageSetting.GetByAcceptLangHeader(acceptlang);
            user.RegisterIp = apiCall.Context.Request.IP;


            var result = GlobalDb.Users.Register2(user, apiCall.GetValue("code"));

            if (result.Response == Data.ViewModel.RegistrationResponse.NeedVerifyCode)
            {
                return new MetaResponse
                {
                    StatusCode = 202,
                    Success = true,
                };
            }
            else if (result.Response == Data.ViewModel.RegistrationResponse.VerifyCodeError)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Verify code Error", apiCall.Context));
            }
            else if (result.Response == Data.ViewModel.RegistrationResponse.InvalidUserNameFormat)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Invalid User name formate", apiCall.Context));
            }
            else if (result.Response == Data.ViewModel.RegistrationResponse.MissingUserNameOrPasswordOrEmail)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Username or password or email not provided", apiCall.Context));
            }
            else if (result.Response == Data.ViewModel.RegistrationResponse.PreservedPrefix)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Reserved user name prefix", apiCall.Context));
            }
            else if (result.Response == Data.ViewModel.RegistrationResponse.UserNameExists)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Username exists", apiCall.Context));
            }
            else if (result.Response == Data.ViewModel.RegistrationResponse.Success && result.User != null)
            {
                string redirct = Kooboo.Web.Service.UserService.GetLoginRedirectUrl(apiCall.Context, result.User, apiCall.Context.Request.Url, null);


                string token = result.User.OneTimeToken;
                apiCall.Context.Response.Headers.Add("access_token", token);
                Kooboo.Data.Cache.AccessTokenCache.SetToken(user.Id, token);

                var response = new MetaResponse();
                response.Success = true;

#if DEBUG 
                redirct = Lib.Helper.UrlHelper.RelativePath(redirct);
#endif

                response.Model = redirct;
                return response;

            }

            throw new Exception(Data.Language.Hardcoded.GetValue("User registration failed", apiCall.Context));

        }

        public virtual MetaResponse OnlineServer(ApiCall call)
        {
            if (call.Context.User == null)
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("User not login", call.Context));
            }

            var user = call.Context.User;
            if (string.IsNullOrWhiteSpace(Data.Service.UserLoginService.GetUserPassword(user)))
            {
                var dbuser = GlobalDb.Users.Get(user.Id);
                if (dbuser != null)
                {
                    user = dbuser;
                }
            }

            var url = Kooboo.Data.Helper.AccountUrlHelper.User("GetMarketServerHost");
            url += "?UserId=" + user.Id.ToString();

            var serverurl = Lib.Helper.HttpHelper.Get<string>(url);

            if (serverurl != null && !serverurl.ToLower().StartsWith("http"))
            {
                serverurl = "https://" + serverurl;
            }

            if (serverurl != null)
            {
                serverurl += "/_admin/market/index";

                var token = Service.UserService.GetTokenFromOnline(user);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    serverurl += "?accesstoken=" + token;
                }

                return new MetaResponse() { RedirectUrl = serverurl, StatusCode = 302 };
            }
            else
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Server url not found", call.Context));
            }

        }

        public bool UpdateProfile(User newuser, ApiCall call)
        {
            var user = call.Context.User;
            user.UserName = newuser.UserName;
            user.Language = newuser.Language;
            user.EmailAddress = newuser.EmailAddress;

            if (GlobalDb.Users.AddOrUpdate(user, call.Context))
            {
                call.Context.User = user;
                return true;
            }
            return false;
        }

        public bool ChangeLanguage(string language, ApiCall call)
        {
            var user = call.Context.User;

            if (user == null)
            {
                throw new Exception("User or Website not valid");
            }

            return GlobalDb.Users.ChangeLanguage(user.Id, language);
        }

        public bool UpdateName(string FirstName, string LastName, ApiCall call)
        {
            var user = call.Context.User;

            if (user == null)
            {
                throw new Exception("User or Website not valid");
            }

            return GlobalDb.Users.UpdateName(user.Id, FirstName, LastName);
        }

        public void SendTelCode(string Tel, ApiCall call)
        {
            PasswordApi password = new PasswordApi();
            password.SmsCode(Tel, call);
        }

        public bool UpdateTel(string Tel, int Code, ApiCall call)
        {
            var user = call.Context.User;

            if (user == null)
            {
                throw new Exception("User or Website not valid");
            }

            return GlobalDb.Users.UpdateTel(user.Id, Tel, Code);
        }

        public bool UpdateEmail(string Email, int Code, ApiCall call)
        {
            var user = call.Context.User;

            if (user == null)
            {
                throw new Exception("User or Website not valid");
            }

            return GlobalDb.Users.UpdateEmail(user.Id, Email, Code);
        }

        public User GetUser(ApiCall call)
        {
            var user = GlobalDb.Users.Profile(call.Context.User.Id);

            if (user != null)
            {
                var org = GlobalDb.Organization.Get(user.CurrentOrgId);
                if (org != null)
                {
                    if (org.AdminUser == user.Id)
                    {
                        user.IsAdmin = true;
                    }
                }
                else
                {
                    user.IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
                }

                if (user.IsAdmin)
                {
                    var adminOrg = GlobalDb.Organization.GetFromAccount(user.Id);
                    if (adminOrg != null)
                    {
                        user.Currency = adminOrg.Currency;
                    }
                }

            }

            return user;
        }

        //  public bool ChangeCurrency(string newCurrency, Guid OrgId, ApiCall call)
        public bool ChangeCurrency(string newCurrency, ApiCall call)
        {
            var url = Kooboo.Data.Helper.AccountUrlHelper.Org("ChangeCurrency");

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("newCurrency", newCurrency);
            return Kooboo.Lib.Helper.HttpHelper.Get2<bool>(
                url,
                data, Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );
        }

        public List<string> AvailableCurrency(ApiCall call)
        {
            List<string> available = new List<string>();
            available.Add("USD");
            available.Add("CNY");
            //available.Add("GBP");
            //available.Add("EUR");
            return available;
        }

        public MetaResponse ChangePassword(string UserName, string OldPassword, string NewPassword, ApiCall call)
        {
            if (GlobalDb.Users.IsDefaultUser(call.Context.User))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Default User can not reset password", call.Context));
            }

            bool isSuccess = GlobalDb.Users.ChangePassword(UserName, OldPassword, NewPassword);
            MetaResponse response = new MetaResponse();
            response.Success = isSuccess;
            return response;
        }

        public bool CheckUser(string username, ApiCall call)
        {
            username = Lib.Helper.StringHelper.ToValidUserNames(username);
            var user = Kooboo.Data.GlobalDb.Users.Get(username);
            return user != null;
        }

        public Dictionary<string, string> Culture(ApiCall call)
        {
            return Kooboo.Data.Language.LanguageSetting.CmsLangs;
        }


        public virtual string ForgotPassword(string Email, ApiCall call)
        {
            string serverulr = GlobalDb.Users.GetServerUrl(Email);

            string url = serverulr + "/_api/User/ForgotPassword";
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Email", Email);
            return Lib.Helper.HttpHelper.Get<string>(url, para);

        }

        public string ResetPassword(Guid token, string password, ApiCall call)
        {
            if (GlobalDb.Users.ResetPassword(token, password))
            {
                return Hardcoded.GetValue("Your password has been reset", call.Context);
            }
            else
            {
                return Hardcoded.GetValue("Password reset failed", call.Context);
            }

        }
        public string GetLanguage(ApiCall call)
        {
            return call.Context.User.Language ?? "en";
        }

        public MetaResponse Logout(ApiCall apiCall)
        {
            var returnUrl = apiCall.GetValue("returnUrl");
            var response = new MetaResponse();
            response.DeleteCookie(DataConstants.UserApiSessionKey);
            response.DeleteCookie(DataConstants.UserJwtToken);

            if (apiCall.WebSite.SsoLogin)
            {
                response.Redirect($"{UrlSetting.SsoLogin}/_api/v2/user/logout?returnUrl={apiCall.Context.Request.Scheme}://{apiCall.Context.Request.Host}{returnUrl}");
            }
            else if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            response.Success = true;
            return response;
        }

        public bool IsUniqueName(string name, ApiCall apiCall)
        {
            name = Lib.Helper.StringHelper.ToValidUserNames(name);

            if (name.Length < 5)
            {
                return false;
            }

            var user = Kooboo.Data.GlobalDb.Users.Get(name);
            if (user != null)
            {
                return false;
            }
            return true;
        }

        public bool IsUniqueEmail(string email, ApiCall call)
        {
            return Kooboo.Data.GlobalDb.Users.GetByEmail(email) == null;
        }

        public MetaResponse SsoLogin(string accessToken, string returnUrl, ApiCall call)
        {
            var res = new MetaResponse();
            res.Redirect(returnUrl);
            call.Context.HttpContext.Response.Cookies.Append(DataConstants.UserJwtToken, accessToken, new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
            return res;
        }
    }
}
