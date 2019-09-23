//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using System;

namespace Kooboo.Web.Service
{
    public static class UserService
    { 
        public static string GetToken(User user)
        {
            if (Kooboo.Data.AppSettings.IsOnlineServer && !IsSameServer(user.TempRedirectUrl))
            {
                string token = GetTokenFromOnline(user);

#if DEBUG
                token = Kooboo.Data.Cache.AccessTokenCache.GetNewToken(user.Id);
#endif

                return token;
            }
            else
            {
                return Kooboo.Data.Cache.AccessTokenCache.GetNewToken(user.Id);
            }
        }

        public static string GetTokenFromOnline(User user)
        {
            var gettokenurl = Kooboo.Data.Helper.AccountUrlHelper.User("GetToken");
            gettokenurl += "?username=" + user.UserName;
            if (user.PasswordHash != default(Guid))
            {
                gettokenurl += "&password=" + user.PasswordHash.ToString();
            }
            else if (!string.IsNullOrEmpty(user.Password))
            {
                gettokenurl += "&password=" + user.Password;
            }
        
            return Kooboo.Lib.Helper.HttpHelper.Get<string>(gettokenurl);
          
        }

        public static string GetRedirectUrl(RenderContext context, User User, string currentRequestUrl, string returnUrl, bool SameSiteRedirect)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                if (!returnUrl.StartsWith("/") && !returnUrl.StartsWith("\\"))
                {
                    returnUrl = "/" + returnUrl;
                }
                if (returnUrl.ToLower().StartsWith("http://") || returnUrl.ToLower().StartsWith("https://"))
                {
                    returnUrl = Lib.Helper.UrlHelper.RelativePath(returnUrl);
                }
            }

            string baseurl = currentRequestUrl; 

            if (!string.IsNullOrWhiteSpace(User.TempRedirectUrl))
            {
                if (Data.AppSettings.IsOnlineServer && !SameSiteRedirect)
                {
                    baseurl = User.TempRedirectUrl;
                }
            }
              
            string url;

            if (string.IsNullOrEmpty(returnUrl))
            {
                context.User = User; 
                url = Kooboo.Data.Service.StartService.AfterLoginPage(context);
            }
            else
            {
                url = returnUrl;
            }

            string fullurl = url; 
           
            if (baseurl !=null && baseurl.ToLower().StartsWith("http://") || baseurl.ToLower().StartsWith("https://"))
            {
                fullurl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url);
            }      
            return fullurl;
        }

        public static string GetLoginRedirectUrl(RenderContext context, User user, string currentrequesturl, string returnurl, bool SameSiteRedirect)
        {
            string redirecturl = GetRedirectUrl(context, user, currentrequesturl, returnurl, SameSiteRedirect);

            string token = GetToken(user);

            return Lib.Helper.UrlHelper.AppendQueryString(redirecturl, "accesstoken", token);
        }

        public static bool IsSameServer(string redirecturl)
        {
            if (string.IsNullOrWhiteSpace(redirecturl))
            {
                return false;
            }

            if (Kooboo.Data.AppSettings.ServerSetting != null)
            {

                var serverid = getServerid(redirecturl);

                return serverid == Kooboo.Data.AppSettings.ServerSetting.ServerId;

            }

            return false;

        }

        public static int getServerid(string redirecturl)
        {
            int index = redirecturl.IndexOf("://");
            if (index > -1)
            {
                redirecturl = redirecturl.Substring(index + 3);
                index = redirecturl.IndexOf(".");
                if (index > -1)
                {
                    redirecturl = redirecturl.Substring(0, index);
                    int serverid;
                    if (!string.IsNullOrWhiteSpace(redirecturl) && int.TryParse(redirecturl, out serverid))
                    {
                        return serverid;
                    }
                }
            }
            return -1;
        }

        public static Guid GuessOrgId(User user, string remotePublishurl)
        {
            remotePublishurl = remotePublishurl.ToLower();
            if (remotePublishurl.StartsWith("https://"))
            {
                remotePublishurl = remotePublishurl.Substring("https://".Length);
            }
            else if (remotePublishurl.StartsWith("http://"))
            {
                remotePublishurl = remotePublishurl.Substring("http://".Length);
            }

            string[] parts = remotePublishurl.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 3 && parts[1].StartsWith("kooboo"))
            {
                var org = parts[0];
                if (!string.IsNullOrEmpty(org))
                {
                    return Lib.Helper.IDHelper.ParseKey(org);
                }
            }

            return user.CurrentOrgId;

        }


    }
}
