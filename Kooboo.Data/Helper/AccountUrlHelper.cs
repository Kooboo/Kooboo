//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Data.Helper
{
    public static class AccountUrlHelper
    {
        public static string AccountBase = AppSettings.AccountApiUrl;

        public static string User(string relativeUrl)
        {
            return AccountBase + "/account/user" + EnsureRelative(relativeUrl);
        }

        public static string UserPublish(string relativeUrl)
        {
            return AccountBase + "/account/userpublish" + EnsureRelative(relativeUrl);
        }

        public static string Org(string relativeUrl)
        {
            return AccountBase + "/account/organization" + EnsureRelative(relativeUrl);
        }

        public static string Cluster(string relativeUrl)
        {
            return AccountBase + "/account/cluster" + EnsureRelative(relativeUrl);
        }

        public static string Domain(string relativeUrl)
        {
            return AccountBase + "/account/domain" + EnsureRelative(relativeUrl);
        }

        public static string System(string relativeUrl)
        {
            return AccountBase + "/account/system" + EnsureRelative(relativeUrl);
        }

        public static string Template(string relativeUrl)
        {
            return AccountBase + "/account/template" + EnsureRelative(relativeUrl);
        }

        public static string Certificate(string relativeUrl)
        {
            return AccountBase + "/account/certificate" + EnsureRelative(relativeUrl);
        }

        public static string Ssl(string relativeUrl)
        {
            return AccountBase + "/account/ssl" + EnsureRelative(relativeUrl);
        }

        public static string OnlineDataCenter(string relativeUrl)
        {
            return AccountBase + "/account/OnlineDataCenter" + EnsureRelative(relativeUrl);
        }

        private static string EnsureRelative(string relativeurl)
        {
            if (string.IsNullOrEmpty(relativeurl))
            {
                return null;
            }
            if (!relativeurl.StartsWith("/"))
            { relativeurl = "/" + relativeurl; }

            return relativeurl;
        }
    }
}